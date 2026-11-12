using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Infraestructura.Seguridad.Criptografia;

/// <summary>
/// Implementación de <see cref="IServicioTokens"/> con JWT: el token de acceso se firma con RS256 y el de renovación
/// con HMAC-SHA256.
/// </summary>
/// <param name="proveedorClaves">Claves y configuración de los tokens.</param>
/// <param name="reloj">Hora actual en UTC, para las fechas de emisión y vencimiento.</param>
internal sealed class ServicioTokens(ProveedorClavesJwt proveedorClaves, IReloj reloj) : IServicioTokens
{
    /// <summary>Bytes de la huella que se conservan (128 bits): suficientes para que no se pueda adivinar.</summary>
    private const int BytesHuella = 16;

    /// <summary>Crea y valida los JWT.</summary>
    private readonly JsonWebTokenHandler _manejador = new();

    /// <summary>
    /// Crea el token de acceso con la identidad, el rol, los permisos, el estado de la contraseña y su huella.
    /// </summary>
    /// <param name="usuario">Cuenta con su rol y permisos cargados.</param>
    /// <returns>El token de acceso y su vencimiento.</returns>
    public TokenEmitidoDto CrearTokenAcceso(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        DateTimeOffset ahora = reloj.AhoraUtc;
        DateTimeOffset vence = ahora.AddMinutes(proveedorClaves.Opciones.MinutosAcceso);

        List<Claim> claims =
        [
            new(NombresClaim.UsuarioId, usuario.Id.ToString(CultureInfo.InvariantCulture)),
            new(NombresClaim.Nombre, usuario.NombreCompleto.Valor),
            new(NombresClaim.Email, usuario.Email.Valor),
            new(NombresClaim.Rol, usuario.Rol.Nombre),
            new(NombresClaim.DebeCambiarContrasena, usuario.DebeCambiarContrasena ? "true" : "false"),
            new(NombresClaim.HuellaContrasena, CalcularHuella(usuario.Contrasena))
        ];
        claims.AddRange(usuario.Rol.Permisos.Select(permiso => new Claim(NombresClaim.Permiso, permiso.Codigo)));

        SecurityTokenDescriptor descriptor = new()
        {
            Issuer = proveedorClaves.Opciones.Emisor,
            Audience = proveedorClaves.Opciones.Audiencia,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = ahora.UtcDateTime,
            NotBefore = ahora.UtcDateTime,
            Expires = vence.UtcDateTime,
            TokenType = TiposToken.Acceso,
            SigningCredentials = new SigningCredentials(proveedorClaves.ClaveFirmaAcceso, SecurityAlgorithms.RsaSha256)
        };

        return new TokenEmitidoDto(_manejador.CreateToken(descriptor), vence);
    }

    /// <summary>
    /// Crea el token de renovación: solo lleva el identificador de la cuenta y la huella de su contraseña.
    /// </summary>
    /// <param name="usuario">Cuenta que inicia o renueva sesión.</param>
    /// <returns>El token de renovación y su vencimiento.</returns>
    public TokenEmitidoDto CrearTokenRenovacion(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        DateTimeOffset ahora = reloj.AhoraUtc;
        DateTimeOffset vence = ahora.AddDays(proveedorClaves.Opciones.DiasRenovacion);

        SecurityTokenDescriptor descriptor = new()
        {
            Issuer = proveedorClaves.Opciones.Emisor,
            Audience = proveedorClaves.Opciones.Emisor,
            Subject = new ClaimsIdentity(
            [
                new Claim(NombresClaim.UsuarioId, usuario.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(NombresClaim.HuellaContrasena, CalcularHuella(usuario.Contrasena))
            ]),
            IssuedAt = ahora.UtcDateTime,
            NotBefore = ahora.UtcDateTime,
            Expires = vence.UtcDateTime,
            TokenType = TiposToken.Renovacion,
            SigningCredentials = new SigningCredentials(proveedorClaves.ClaveRenovacion, SecurityAlgorithms.HmacSha256)
        };

        return new TokenEmitidoDto(_manejador.CreateToken(descriptor), vence);
    }

    /// <summary>
    /// Verifica firma, emisor, destinatario, tipo y vencimiento de un token de renovación, y extrae sus datos.
    /// </summary>
    /// <param name="token">Token recibido en la cookie de renovación.</param>
    /// <returns>Los datos del token, o <see langword="null"/> si no es válido.</returns>
    public async Task<TokenRenovacionValidoDto?> ValidarTokenRenovacionAsync(string token)
    {
        TokenValidationParameters parametros = new()
        {
            ValidIssuer = proveedorClaves.Opciones.Emisor,
            ValidAudience = proveedorClaves.Opciones.Emisor,
            IssuerSigningKey = proveedorClaves.ClaveRenovacion,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ValidTypes = [TiposToken.Renovacion],
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        TokenValidationResult resultado = await _manejador.ValidateTokenAsync(token, parametros);
        if (!resultado.IsValid ||
            !resultado.Claims.TryGetValue(NombresClaim.UsuarioId, out object? usuarioId) ||
            !resultado.Claims.TryGetValue(NombresClaim.HuellaContrasena, out object? huella) ||
            !int.TryParse(usuarioId?.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out int id) ||
            huella is not string huellaTexto)
        {
            return null;
        }

        return new TokenRenovacionValidoDto(id, huellaTexto);
    }

    /// <summary>
    /// Compara en tiempo constante la huella del token con la de la contraseña actual, para no filtrar información por el
    /// tiempo de respuesta.
    /// </summary>
    /// <param name="usuario">Cuenta con su hash de contraseña actual.</param>
    /// <param name="huella">Huella que trae el token.</param>
    /// <returns><see langword="true"/> si corresponde a la contraseña actual.</returns>
    public bool EsHuellaVigente(Usuario usuario, string huella)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(CalcularHuella(usuario.Contrasena)),
            Encoding.ASCII.GetBytes(huella ?? string.Empty));
    }

    /// <summary>
    /// Calcula la huella de un hash de contraseña: HMAC-SHA256 con la clave de huella, recortado a 128 bits y en Base64Url.
    /// No revela el hash, y sin la clave no se puede calcular.
    /// </summary>
    /// <param name="hashContrasena">Hash de la contraseña guardado en la cuenta.</param>
    /// <returns>La huella.</returns>
    private string CalcularHuella(string hashContrasena)
    {
        byte[] firma = HMACSHA256.HashData(proveedorClaves.ClaveHuella, Encoding.UTF8.GetBytes(hashContrasena));
        return Base64UrlEncoder.Encode(firma.AsSpan(0, BytesHuella).ToArray());
    }
}
