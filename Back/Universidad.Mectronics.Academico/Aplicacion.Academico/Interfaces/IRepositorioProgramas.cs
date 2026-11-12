namespace Aplicacion.Academico.Interfaces;

/// <summary>
/// Acceso de solo lectura a los programas académicos.
/// </summary>
public interface IRepositorioProgramas
{
    /// <summary>
    /// Indica si existe un programa activo.
    /// </summary>
    /// <param name="id">Identificador del programa.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si existe y está activo.</returns>
    Task<bool> ExisteActivoAsync(int id, CancellationToken cancelacion);
}
