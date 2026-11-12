using Universidad.Mectronics.Transversal.Auditoria;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>
/// Auditoría simulada: descarta los eventos, porque las pruebas no verifican qué se registró.
/// </summary>
public sealed class RegistroEventosFalso : IRegistroEventos
{
    /// <summary>No hace nada.</summary>
    /// <param name="mensaje">Mensaje.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Una tarea completada.</returns>
    public Task RegistrarInformacionAsync(string mensaje, CancellationToken cancelacion) => Task.CompletedTask;

    /// <summary>No hace nada.</summary>
    /// <param name="mensaje">Mensaje.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Una tarea completada.</returns>
    public Task RegistrarAdvertenciaAsync(string mensaje, CancellationToken cancelacion) => Task.CompletedTask;

    /// <summary>No hace nada.</summary>
    /// <param name="mensaje">Mensaje.</param>
    /// <param name="excepcion">Excepción.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Una tarea completada.</returns>
    public Task RegistrarErrorAsync(string mensaje, Exception excepcion, CancellationToken cancelacion) => Task.CompletedTask;
}
