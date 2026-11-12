namespace Universidad.Mectronics.Transversal.Auditoria;

/// <summary>Registro de eventos de auditoría y errores.</summary>
public interface IRegistroEventos
{
    /// <summary>
    /// Registra un evento informativo (por ejemplo, un inicio de sesión exitoso o la creación de una cuenta).
    /// </summary>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando el evento queda pendiente de envío.</returns>
    Task RegistrarInformacionAsync(string mensaje, CancellationToken cancelacion);

    /// <summary>
    /// Registra una advertencia (por ejemplo, un inicio de sesión fallido o una cuenta bloqueada).
    /// </summary>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando el evento queda pendiente de envío.</returns>
    Task RegistrarAdvertenciaAsync(string mensaje, CancellationToken cancelacion);

    /// <summary>
    /// Registra un error inesperado con el detalle técnico de la excepción.
    /// </summary>
    /// <param name="mensaje">Descripción del error.</param>
    /// <param name="excepcion">Excepción que lo causó; su detalle se guarda en la columna <c>Excepcion</c>.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando el error queda pendiente de envío.</returns>
    Task RegistrarErrorAsync(string mensaje, Exception excepcion, CancellationToken cancelacion);
}
