using Aplicacion.Seguridad.Dtos.Autenticacion;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Autenticacion;

/// <summary>
/// Casos de uso de inicio y renovación de sesión. Lo consume la API, que entrega los tokens en cookies.
/// </summary>
public interface IServicioAutenticacion
{
    /// <summary>
    /// Verifica las credenciales y, si son correctas, crea una sesión. Registra los intentos fallidos y bloquea la cuenta
    /// al superar el máximo permitido.
    /// </summary>
    /// <param name="datos">Correo y contraseña escritos por el usuario.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La sesión creada, o un error de validación o de credenciales inválidas.</returns>
    Task<Resultado<SesionCreadaDto>> IniciarSesionAsync(IniciarSesionDto datos, CancellationToken cancelacion);

    /// <summary>
    /// Crea una sesión nueva a partir de un token de renovación válido y rota el token de renovación.
    /// </summary>
    /// <param name="datos">Token de renovación recibido en la cookie.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La sesión renovada, o un error de sesión inválida.</returns>
    Task<Resultado<SesionCreadaDto>> RenovarSesionAsync(RenovarSesionDto datos, CancellationToken cancelacion);
}
