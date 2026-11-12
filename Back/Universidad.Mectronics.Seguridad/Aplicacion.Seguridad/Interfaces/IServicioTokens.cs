using Aplicacion.Seguridad.Dtos.Autenticacion;
using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Interfaces;

/// <summary>Crea y valida los tokens de acceso y de renovación.</summary>
public interface IServicioTokens
{
    /// <summary>
    /// Crea el token de acceso con la identidad, el rol, los permisos de la cuenta y la huella de su contraseña.
    /// </summary>
    /// <param name="usuario">Cuenta que inicia o renueva sesión, con su rol y permisos cargados.</param>
    /// <returns>El token de acceso y su vencimiento.</returns>
    TokenEmitidoDto CrearTokenAcceso(Usuario usuario);

    /// <summary>
    /// Crea el token de renovación, firmado con una clave distinta a la del token de acceso.
    /// </summary>
    /// <param name="usuario">Cuenta que inicia o renueva sesión.</param>
    /// <returns>El token de renovación y su vencimiento.</returns>
    TokenEmitidoDto CrearTokenRenovacion(Usuario usuario);

    /// <summary>
    /// Verifica la firma y el vencimiento de un token de renovación y extrae sus datos.
    /// </summary>
    /// <param name="token">Token recibido en la cookie de renovación.</param>
    /// <returns>Los datos del token, o <see langword="null"/> si no es válido o ya venció.</returns>
    Task<TokenRenovacionValidoDto?> ValidarTokenRenovacionAsync(string token);

    /// <summary>
    /// Indica si la huella corresponde a la contraseña actual de la cuenta. Cambiar la contraseña invalida los tokens
    /// anteriores.
    /// </summary>
    /// <param name="usuario">Cuenta con su hash de contraseña actual.</param>
    /// <param name="huella">Huella que trae el token.</param>
    /// <returns><see langword="true"/> si la huella corresponde a la contraseña actual.</returns>
    bool EsHuellaVigente(Usuario usuario, string huella);
}
