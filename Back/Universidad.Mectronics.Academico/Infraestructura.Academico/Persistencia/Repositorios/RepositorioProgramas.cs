using Aplicacion.Academico.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Academico.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IRepositorioProgramas"/> con Entity Framework.
/// </summary>
/// <param name="contexto">Contexto de la petición.</param>
internal sealed class RepositorioProgramas(ContextoAcademico contexto) : IRepositorioProgramas
{
    /// <summary>
    /// Indica si existe un programa activo.
    /// </summary>
    /// <param name="id">Identificador del programa.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si existe y está activo.</returns>
    public Task<bool> ExisteActivoAsync(int id, CancellationToken cancelacion) =>
        contexto.Programas.AnyAsync(programa => programa.Id == id && programa.Activo, cancelacion);
}
