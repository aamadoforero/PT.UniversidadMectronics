namespace Aplicacion.Logs.Interfaces;

/// <summary>
/// Confirma en la base de datos los cambios pendientes de los repositorios.
/// </summary>
public interface IUnidadTrabajo
{
    /// <summary>
    /// Guarda los cambios pendientes.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El número de registros afectados.</returns>
    Task<int> GuardarCambiosAsync(CancellationToken cancelacion);
}
