using Aplicacion.Seguridad.Dtos.Catalogos;

namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Consultas de solo lectura sobre los catálogos académicos que Seguridad necesita para crear y editar perfiles:
/// programas y categorías de profesor. Seguridad no modifica estos catálogos. Lo implementa Infraestructura.
/// </summary>
public interface IConsultaCatalogos
{
    /// <summary>
    /// Obtiene los programas académicos activos, ordenados por nombre.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Los programas activos.</returns>
    Task<IReadOnlyList<ElementoCatalogoDto>> ObtenerProgramasActivosAsync(CancellationToken cancelacion);

    /// <summary>
    /// Obtiene las categorías de profesor, ordenadas por nombre.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Las categorías de profesor.</returns>
    Task<IReadOnlyList<ElementoCatalogoDto>> ObtenerCategoriasProfesorAsync(CancellationToken cancelacion);

    /// <summary>
    /// Indica si existe un programa académico activo con el identificador indicado.
    /// </summary>
    /// <param name="programaId">Identificador del programa.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si el programa existe y está activo.</returns>
    Task<bool> ExisteProgramaActivoAsync(int programaId, CancellationToken cancelacion);

    /// <summary>
    /// Indica si existe una categoría de profesor con el identificador indicado.
    /// </summary>
    /// <param name="categoriaId">Identificador de la categoría.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si la categoría existe.</returns>
    Task<bool> ExisteCategoriaProfesorAsync(byte categoriaId, CancellationToken cancelacion);
}
