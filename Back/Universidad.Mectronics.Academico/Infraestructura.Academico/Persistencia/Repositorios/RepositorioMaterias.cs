using Aplicacion.Academico.Interfaces;
using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Academico.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IRepositorioMaterias"/> con Entity Framework.
/// </summary>
/// <param name="contexto">Contexto de la petición.</param>
internal sealed class RepositorioMaterias(ContextoAcademico contexto) : IRepositorioMaterias
{
    /// <summary>
    /// Obtiene una materia con su profesor, en seguimiento.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La materia, o <see langword="null"/> si no existe.</returns>
    public Task<Materia?> ObtenerPorIdAsync(int id, CancellationToken cancelacion) =>
        contexto.Materias
            .Include(materia => materia.Profesor)
                .ThenInclude(profesor => profesor.Usuario)
            .FirstOrDefaultAsync(materia => materia.Id == id, cancelacion);

    /// <summary>
    /// Obtiene las materias con su profesor, ordenadas por nombre.
    /// </summary>
    /// <param name="incluirInactivas">Si se incluyen las desactivadas.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Las materias.</returns>
    public async Task<IReadOnlyList<Materia>> ObtenerTodasAsync(bool incluirInactivas, CancellationToken cancelacion) =>
        await contexto.Materias
            .AsNoTracking()
            .Where(materia => incluirInactivas || materia.Activo)
            .Include(materia => materia.Profesor)
                .ThenInclude(profesor => profesor.Usuario)
            .OrderBy(materia => materia.Nombre)
            .ToListAsync(cancelacion);

    /// <summary>
    /// Obtiene las materias activas de un profesor; usa el índice <c>IX_Materia_ProfesorId</c>.
    /// </summary>
    /// <param name="profesorId">Profesor.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Las materias, ordenadas por nombre.</returns>
    public async Task<IReadOnlyList<Materia>> ObtenerActivasPorProfesorAsync(int profesorId, CancellationToken cancelacion) =>
        await contexto.Materias
            .AsNoTracking()
            .Where(materia => materia.ProfesorId == profesorId && materia.Activo)
            .OrderBy(materia => materia.Nombre)
            .ToListAsync(cancelacion);

    /// <summary>
    /// Cuenta las materias activas de un profesor.
    /// </summary>
    /// <param name="profesorId">Profesor.</param>
    /// <param name="materiaExcluidaId">Materia que no se cuenta, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El número de materias activas.</returns>
    public Task<int> ContarActivasPorProfesorAsync(int profesorId, int? materiaExcluidaId, CancellationToken cancelacion) =>
        contexto.Materias.CountAsync(
            materia => materia.ProfesorId == profesorId && materia.Activo && materia.Id != materiaExcluidaId,
            cancelacion);

    /// <summary>
    /// Obtiene cuántas materias activas dicta cada profesor.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El número de materias activas por profesor.</returns>
    public async Task<IReadOnlyDictionary<int, int>> ObtenerConteoActivasPorProfesorAsync(CancellationToken cancelacion) =>
        await contexto.Materias
            .Where(materia => materia.Activo)
            .GroupBy(materia => materia.ProfesorId)
            .Select(grupo => new { ProfesorId = grupo.Key, Total = grupo.Count() })
            .ToDictionaryAsync(fila => fila.ProfesorId, fila => fila.Total, cancelacion);

    /// <summary>
    /// Indica si el programa ya tiene una materia con ese nombre (la intercalación de la base no distingue mayúsculas).
    /// </summary>
    /// <param name="programaId">Programa.</param>
    /// <param name="nombre">Nombre a buscar.</param>
    /// <param name="materiaExcluidaId">Materia que no se compara, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si el nombre ya está en uso.</returns>
    public Task<bool> ExisteNombreAsync(int programaId, string nombre, int? materiaExcluidaId, CancellationToken cancelacion) =>
        contexto.Materias.AnyAsync(
            materia => materia.ProgramaId == programaId && materia.Nombre == nombre && materia.Id != materiaExcluidaId,
            cancelacion);

    /// <summary>
    /// Cuenta los estudiantes con inscripción vigente en una materia.
    /// </summary>
    /// <param name="materiaId">Materia.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El número de inscritos.</returns>
    public Task<int> ContarInscritosAsync(int materiaId, CancellationToken cancelacion) =>
        contexto.Inscripciones.CountAsync(inscripcion => inscripcion.MateriaId == materiaId && inscripcion.Activo, cancelacion);

    /// <summary>
    /// Agrega una materia nueva al contexto.
    /// </summary>
    /// <param name="materia">Materia a crear.</param>
    public void Agregar(Materia materia) => contexto.Materias.Add(materia);
}
