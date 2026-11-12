using System.Security.Cryptography;
using Infraestructura.Seguridad.Configuracion;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Infraestructura.Seguridad.Criptografia;

/// <summary>
/// Carga las claves de <see cref="OpcionesJwt"/> y publica la parte pública de la clave RSA (JWKS).
/// </summary>
public sealed class ProveedorClavesJwt : IDisposable
{
    /// <summary>Clave RSA con la parte privada; solo existe en este microservicio.</summary>
    private readonly RSA _rsa;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="ProveedorClavesJwt"/>.</summary>
    /// <param name="opciones">Configuración de los tokens.</param>
    public ProveedorClavesJwt(IOptions<OpcionesJwt> opciones)
    {
        ArgumentNullException.ThrowIfNull(opciones);
        Opciones = opciones.Value;

        _rsa = RSA.Create();
        _rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(Opciones.ClavePrivadaAcceso), out _);

        RsaSecurityKey clavePublica = new(_rsa.ExportParameters(includePrivateParameters: false));
        string identificadorClave = Base64UrlEncoder.Encode(clavePublica.ComputeJwkThumbprint());

        ClaveFirmaAcceso = new RsaSecurityKey(_rsa) { KeyId = identificadorClave };
        ClavePublicaAcceso = new RsaSecurityKey(_rsa.ExportParameters(includePrivateParameters: false)) { KeyId = identificadorClave };
        ClaveRenovacion = new SymmetricSecurityKey(Convert.FromBase64String(Opciones.ClaveRenovacion));
        ClaveHuella = Convert.FromBase64String(Opciones.ClaveHuella);
    }

    /// <summary>Configuración de los tokens (emisor, audiencia y vigencias).</summary>
    public OpcionesJwt Opciones { get; }

    /// <summary>Clave privada RSA con que se firman los tokens de acceso (RS256).</summary>
    public RsaSecurityKey ClaveFirmaAcceso { get; }

    /// <summary>Parte pública de la clave RSA, con la que se verifican los tokens de acceso.</summary>
    public RsaSecurityKey ClavePublicaAcceso { get; }

    /// <summary>Clave simétrica con que se firman y verifican los tokens de renovación (HMAC-SHA256).</summary>
    public SymmetricSecurityKey ClaveRenovacion { get; }

    /// <summary>Clave con que se calcula la huella de la contraseña (HMAC-SHA256).</summary>
    public byte[] ClaveHuella { get; }

    /// <summary>
    /// Crea los parámetros para validar un token de acceso: firma RS256, emisor, audiencia, tipo <c>at+jwt</c> y vencimiento.
    /// Los usa la API para autenticar cada petición.
    /// </summary>
    /// <returns>Los parámetros de validación.</returns>
    public TokenValidationParameters CrearParametrosValidacionAcceso() => new()
    {
        ValidIssuer = Opciones.Emisor,
        ValidAudience = Opciones.Audiencia,
        IssuerSigningKey = ClavePublicaAcceso,
        ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
        ValidTypes = [TiposToken.Acceso],
        NameClaimType = NombresClaim.Nombre,
        RoleClaimType = NombresClaim.Rol,
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    /// <summary>
    /// Obtiene el conjunto de claves públicas en formato JWKS (RFC 7517), para publicarlo en <c>/.well-known/jwks.json</c>.
    /// Nunca incluye la parte privada.
    /// </summary>
    /// <returns>El conjunto con la clave pública de los tokens de acceso.</returns>
    public JsonWebKeySet ObtenerConjuntoClavesPublicas()
    {
        JsonWebKey clave = JsonWebKeyConverter.ConvertFromRSASecurityKey(ClavePublicaAcceso);
        clave.Use = "sig";
        clave.Alg = SecurityAlgorithms.RsaSha256;

        JsonWebKeySet conjunto = new();
        conjunto.Keys.Add(clave);
        return conjunto;
    }

    /// <summary>
    /// Libera la clave RSA al cerrar la aplicación.
    /// </summary>
    public void Dispose() => _rsa.Dispose();
}
