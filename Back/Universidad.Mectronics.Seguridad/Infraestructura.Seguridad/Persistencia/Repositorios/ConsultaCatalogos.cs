using Aplicacion.Seguridad.Dtos.Catalogos;
using Aplicacion.Seguridad.Interfaces;
using Infraestructura.Seguridad.Persistencia.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Seguridad.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IConsultaCatalogos"/> con consultas de solo lectura sobre los catálogos académicos.
/// </summary>
/// <param name="contexto">Contexto de la petición en curso.</param>
internal sealed class ConsultaCatalogos(ContextoSeguridad contexto) : IConsultaCatalogos
{
    /// <summary>
    /// Obtiene los programas activos, ordenados por nombre.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Los programas activos.</returns>
    public async Task<IReadOnlyList<ElementoCatalogoDto>> ObtenerProgramasActivosAsync(CancellationToken cancelacion) =>
        await contexto.Set<ProgramaCatalogo>()
            .AsNoTracking()
            .Where(programa => programa.Activo)
            .OrderBy(programa => programa.Nombre)
            .Select(programa => new ElementoCatalogoDto(programa.Id, programa.Nombre))
            .ToListAsync(cancelacion);

    /// <summary>
    /// Obtiene las categorías de profesor, ordenadas por nombre.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Las categorías.</returns>
    public async Task<IReadOnlyList<ElementoCatalogoDto>> ObtenerCategoriasProfesorAsync(CancellationToken cancelacion) =>
        await contexto.Set<CategoriaProfesorCatalogo>()
            .AsNoTracking()
            .OrderBy(categoria => categoria.Nombre)
            .Select(categoria => new ElementoCatalogoDto(categoria.Id, categoria.Nombre))
            .ToListAsync(cancelacion);

    /// <summary>
    /// Indica si existe un programa activo con el identificador indicado.
    /// </summary>
    /// <param name="programaId">Identificador del programa.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si el programa existe y está activo.</returns>
    public Task<bool> ExisteProgramaActivoAsync(int programaId, CancellationToken cancelacion) =>
        contexto.Set<ProgramaCatalogo>().AnyAsync(programa => programa.Id == programaId && programa.Activo, cancelacion);

    /// <summary>
    /// Indica si existe una categoría de profesor con el identificador indicado.
    /// </summary>
    /// <param name="categoriaId">Identificador de la categoría.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si la categoría existe.</returns>
    public Task<bool> ExisteCategoriaProfesorAsync(byte categoriaId, CancellationToken cancelacion) =>
        contexto.Set<CategoriaProfesorCatalogo>().AnyAsync(categoria => categoria.Id == categoriaId, cancelacion);
}
