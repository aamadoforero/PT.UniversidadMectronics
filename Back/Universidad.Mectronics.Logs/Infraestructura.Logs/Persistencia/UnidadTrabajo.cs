using Aplicacion.Logs.Interfaces;

namespace Infraestructura.Logs.Persistencia;

/// <summary>
/// Implementación de <see cref="IUnidadTrabajo"/> sobre el contexto de Entity Framework.
/// </summary>
/// <param name="contexto">Contexto de la petición.</param>
internal sealed class UnidadTrabajo(ContextoLogs contexto) : IUnidadTrabajo
{
    /// <summary>
    /// Guarda los cambios pendientes.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El número de registros afectados.</returns>
    public Task<int> GuardarCambiosAsync(CancellationToken cancelacion) => contexto.SaveChangesAsync(cancelacion);
}
