using Aplicacion.Academico.Interfaces;

namespace PruebasUnitarias.Academico.Simuladores;

/// <summary>
/// Repositorio de programas en memoria: solo conoce los identificadores de los programas activos.
/// </summary>
/// <param name="programasActivos">Identificadores de los programas activos.</param>
public sealed class RepositorioProgramasEnMemoria(params int[] programasActivos) : IRepositorioProgramas
{
    /// <summary>
    /// Indica si el programa está en la lista de activos.
    /// </summary>
    /// <param name="id">Identificador del programa.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns><see langword="true"/> si está activo.</returns>
    public Task<bool> ExisteActivoAsync(int id, CancellationToken cancelacion) => Task.FromResult(programasActivos.Contains(id));
}
