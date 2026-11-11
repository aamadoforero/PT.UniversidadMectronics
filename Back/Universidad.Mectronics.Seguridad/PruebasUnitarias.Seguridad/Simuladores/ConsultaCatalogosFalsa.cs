using Aplicacion.Seguridad.Dtos.Catalogos;
using Aplicacion.Seguridad.Interfaces;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>
/// Catálogos simulados: existe el programa 1 y las categorías 1 a 3, como en la semilla de la base de datos.
/// </summary>
public sealed class ConsultaCatalogosFalsa : IConsultaCatalogos
{
    /// <summary>Devuelve el programa simulado.</summary>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Los programas.</returns>
    public Task<IReadOnlyList<ElementoCatalogoDto>> ObtenerProgramasActivosAsync(CancellationToken cancelacion) =>
        Task.FromResult<IReadOnlyList<ElementoCatalogoDto>>([new ElementoCatalogoDto(1, "Ingeniería de Sistemas")]);

    /// <summary>Devuelve las categorías simuladas.</summary>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Las categorías.</returns>
    public Task<IReadOnlyList<ElementoCatalogoDto>> ObtenerCategoriasProfesorAsync(CancellationToken cancelacion) =>
        Task.FromResult<IReadOnlyList<ElementoCatalogoDto>>([new(1, "Planta"), new(2, "Cátedra"), new(3, "Ocasional")]);

    /// <summary>Indica si el programa existe.</summary>
    /// <param name="programaId">Identificador.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns><see langword="true"/> solo para el programa 1.</returns>
    public Task<bool> ExisteProgramaActivoAsync(int programaId, CancellationToken cancelacion) => Task.FromResult(programaId == 1);

    /// <summary>Indica si la categoría existe.</summary>
    /// <param name="categoriaId">Identificador.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns><see langword="true"/> para las categorías 1 a 3.</returns>
    public Task<bool> ExisteCategoriaProfesorAsync(byte categoriaId, CancellationToken cancelacion) =>
        Task.FromResult(categoriaId is >= 1 and <= 3);
}
