using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>
/// Tokens simulados que llevan el identificador de la cuenta y su hash como huella.
/// </summary>
public sealed class ServicioTokensFalso : IServicioTokens
{
    /// <summary>Contador para que cada token emitido sea distinto.</summary>
    private int _emitidos;

    /// <summary>
    /// Crea un token de acceso simulado.
    /// </summary>
    /// <param name="usuario">Cuenta.</param>
    /// <returns>El token simulado.</returns>
    public TokenEmitidoDto CrearTokenAcceso(Usuario usuario) =>
        new($"acceso|{usuario.Id}|{++_emitidos}", DateTimeOffset.MaxValue);

    /// <summary>
    /// Crea un token de renovación simulado con la huella de la contraseña.
    /// </summary>
    /// <param name="usuario">Cuenta.</param>
    /// <returns>El token simulado.</returns>
    public TokenEmitidoDto CrearTokenRenovacion(Usuario usuario) =>
        new($"renovacion|{usuario.Id}|{usuario.Contrasena}|{++_emitidos}", DateTimeOffset.MaxValue);

    /// <summary>
    /// Lee un token de renovación simulado.
    /// </summary>
    /// <param name="token">Token recibido.</param>
    /// <returns>Sus datos, o <see langword="null"/> si no tiene el formato esperado.</returns>
    public Task<TokenRenovacionValidoDto?> ValidarTokenRenovacionAsync(string token)
    {
        string[] partes = token.Split('|');
        TokenRenovacionValidoDto? datos = partes is ["renovacion", string id, string huella, _]
            ? new TokenRenovacionValidoDto(int.Parse(id, System.Globalization.CultureInfo.InvariantCulture), huella)
            : null;
        return Task.FromResult(datos);
    }

    /// <summary>
    /// Compara la huella simulada con el hash actual.
    /// </summary>
    /// <param name="usuario">Cuenta.</param>
    /// <param name="huella">Huella del token.</param>
    /// <returns><see langword="true"/> si la contraseña no cambió.</returns>
    public bool EsHuellaVigente(Usuario usuario, string huella) => usuario.Contrasena == huella;
}
