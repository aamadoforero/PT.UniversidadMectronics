using Aplicacion.Academico.Interfaces;
using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Academico.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IRepositorioProfesores"/> con Entity Framework.
/// </summary>
/// <param name="contexto">Contexto de la petición.</param>
internal sealed class RepositorioProfesores(ContextoAcademico contexto) : IRepositorioProfesores
{
    /// <summary>
    /// Obtiene los profesores con cuenta activa, ordenados por nombre.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Los profesores.</returns>
    public async Task<IReadOnlyList<Profesor>> ObtenerTodosAsync(CancellationToken cancelacion) =>
        await contexto.Profesores
            .AsNoTracking()
            .Include(profesor => profesor.Usuario)
            .Where(profesor => profesor.Usuario.Activo)
            .OrderBy(profesor => profesor.Usuario.NombreCompleto)
            .ToListAsync(cancelacion);

    /// <summary>
    /// Obtiene un profesor con cuenta activa.
    /// </summary>
    /// <param name="id">Identificador del perfil de profesor.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El profesor, o <see langword="null"/> si no existe o su cuenta está inactiva.</returns>
    public Task<Profesor?> ObtenerActivoPorIdAsync(int id, CancellationToken cancelacion) =>
        contexto.Profesores
            .Include(profesor => profesor.Usuario)
            .FirstOrDefaultAsync(profesor => profesor.Id == id && profesor.Usuario.Activo, cancelacion);

    /// <summary>
    /// Obtiene el profesor de una cuenta.
    /// </summary>
    /// <param name="usuarioId">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El profesor, o <see langword="null"/> si la cuenta no tiene perfil de profesor.</returns>
    public Task<Profesor?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancelacion) =>
        contexto.Profesores
            .AsNoTracking()
            .Include(profesor => profesor.Usuario)
            .FirstOrDefaultAsync(profesor => profesor.UsuarioId == usuarioId, cancelacion);
}
