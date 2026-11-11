namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Registra eventos de auditoría y errores en la tabla <c>Logs.RegistroEvento</c>, con la aplicación
/// <c>Servicio-Seguridad</c>. Lo implementa Infraestructura; si el registro falla, no debe interrumpir la operación principal.
/// </summary>
public interface IRegistroEventos
{
    /// <summary>
    /// Registra un evento informativo (por ejemplo, un inicio de sesión exitoso o la creación de una cuenta).
    /// </summary>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando el evento queda registrado.</returns>
    Task RegistrarInformacionAsync(string mensaje, CancellationToken cancelacion);

    /// <summary>
    /// Registra una advertencia (por ejemplo, un inicio de sesión fallido o una cuenta bloqueada).
    /// </summary>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando el evento queda registrado.</returns>
    Task RegistrarAdvertenciaAsync(string mensaje, CancellationToken cancelacion);

    /// <summary>
    /// Registra un error inesperado con el detalle técnico de la excepción.
    /// </summary>
    /// <param name="mensaje">Descripción del error.</param>
    /// <param name="excepcion">Excepción que lo causó; su detalle se guarda en la columna <c>Excepcion</c>.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando el error queda registrado.</returns>
    Task RegistrarErrorAsync(string mensaje, Exception excepcion, CancellationToken cancelacion);
}
