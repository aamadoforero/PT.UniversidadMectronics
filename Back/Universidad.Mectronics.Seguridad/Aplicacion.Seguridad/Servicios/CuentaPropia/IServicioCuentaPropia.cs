using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Dtos.CuentaPropia;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.CuentaPropia;

/// <summary>
/// Casos de uso que el usuario autenticado realiza sobre su propia cuenta. Siempre actúan sobre la cuenta de quien hace
/// la petición, nunca sobre otra, así que no reciben un identificador.
/// </summary>
public interface IServicioCuentaPropia
{
    /// <summary>
    /// Obtiene el perfil completo de la cuenta autenticada.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El perfil, o un error de sesión inválida si la cuenta ya no existe o está inactiva.</returns>
    Task<Resultado<PerfilDto>> ObtenerPerfilAsync(CancellationToken cancelacion);

    /// <summary>
    /// Actualiza los datos que el propio usuario puede modificar (su nombre).
    /// </summary>
    /// <param name="datos">Datos nuevos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El perfil actualizado, o un error de validación o de sesión inválida.</returns>
    Task<Resultado<PerfilDto>> ActualizarDatosPersonalesAsync(ActualizarDatosPersonalesDto datos, CancellationToken cancelacion);

    /// <summary>
    /// Cambia la contraseña verificando la actual. Como la huella de la contraseña cambia, los tokens anteriores quedan
    /// invalidados y se entrega una sesión nueva.
    /// </summary>
    /// <param name="datos">Contraseña actual y contraseña nueva.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La sesión nueva, o un error de validación o de sesión inválida.</returns>
    Task<Resultado<SesionCreadaDto>> CambiarContrasenaAsync(CambiarContrasenaDto datos, CancellationToken cancelacion);
}
