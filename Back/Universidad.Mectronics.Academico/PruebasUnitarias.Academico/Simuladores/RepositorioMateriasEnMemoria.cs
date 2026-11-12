using Aplicacion.Academico.Interfaces;
using Dominio.Academico.Entidades;
using PruebasUnitarias.Academico.DatosMemoria;

namespace PruebasUnitarias.Academico.Simuladores;

/// <summary>
/// Repositorio de materias en memoria. Cuenta los inscritos a partir de los estudiantes del repositorio de estudiantes.
/// </summary>
/// <param name="estudiantes">Estudiantes, para contar inscritos.</param>
/// <param name="materias">Materias iniciales.</param>
public sealed class RepositorioMateriasEnMemoria(RepositorioEstudiantesEnMemoria estudiantes, IEnumerable<Materia> materias) : IRepositorioMaterias
{
    /// <summary>Materias guardadas.</summary>
    public List<Materia> Materias { get; } = [.. materias];

    /// <summary>
    /// Busca por identificador.
    /// </summary>
    /// <param name="id">Identificador.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>La materia, o <see langword="null"/>.</returns>
    public Task<Materia?> ObtenerPorIdAsync(int id, CancellationToken cancelacion) =>
        Task.FromResult(Materias.FirstOrDefault(materia => materia.Id == id));

    /// <summary>
    /// Obtiene las materias.
    /// </summary>
    /// <param name="incluirInactivas">Si se incluyen las desactivadas.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Las materias, ordenadas por nombre.</returns>
    public Task<IReadOnlyList<Materia>> ObtenerTodasAsync(bool incluirInactivas, CancellationToken cancelacion)
    {
        IReadOnlyList<Materia> resultado = Materias
            .Where(materia => incluirInactivas || materia.Activo)
            .OrderBy(materia => materia.Nombre, StringComparer.Ordinal)
            .ToList();
        return Task.FromResult(resultado);
    }

    /// <summary>
    /// Obtiene las materias activas del profesor.
    /// </summary>
    /// <param name="profesorId">Profesor.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Las materias, ordenadas por nombre.</returns>
    public Task<IReadOnlyList<Materia>> ObtenerActivasPorProfesorAsync(int profesorId, CancellationToken cancelacion)
    {
        IReadOnlyList<Materia> resultado = Materias
            .Where(materia => materia.ProfesorId == profesorId && materia.Activo)
            .OrderBy(materia => materia.Nombre, StringComparer.Ordinal)
            .ToList();
        return Task.FromResult(resultado);
    }

    /// <summary>
    /// Cuenta las materias activas del profesor.
    /// </summary>
    /// <param name="profesorId">Profesor.</param>
    /// <param name="materiaExcluidaId">Materia que no se cuenta.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>El conteo.</returns>
    public Task<int> ContarActivasPorProfesorAsync(int profesorId, int? materiaExcluidaId, CancellationToken cancelacion) =>
        Task.FromResult(Materias.Count(materia => materia.ProfesorId == profesorId && materia.Activo && materia.Id != materiaExcluidaId));

    /// <summary>
    /// Agrupa las materias activas por profesor.
    /// </summary>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>El conteo por profesor.</returns>
    public Task<IReadOnlyDictionary<int, int>> ObtenerConteoActivasPorProfesorAsync(CancellationToken cancelacion)
    {
        IReadOnlyDictionary<int, int> conteo = Materias
            .Where(materia => materia.Activo)
            .GroupBy(materia => materia.ProfesorId)
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Count());
        return Task.FromResult(conteo);
    }

    /// <summary>
    /// Busca el nombre sin distinguir mayúsculas, como la intercalación de la base de datos.
    /// </summary>
    /// <param name="programaId">Programa.</param>
    /// <param name="nombre">Nombre.</param>
    /// <param name="materiaExcluidaId">Materia que no se compara.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns><see langword="true"/> si el nombre ya existe.</returns>
    public Task<bool> ExisteNombreAsync(int programaId, string nombre, int? materiaExcluidaId, CancellationToken cancelacion) =>
        Task.FromResult(Materias.Any(materia => materia.ProgramaId == programaId &&
                                                string.Equals(materia.Nombre, nombre, StringComparison.OrdinalIgnoreCase) &&
                                                materia.Id != materiaExcluidaId));

    /// <summary>
    /// Cuenta los estudiantes que tienen inscrita la materia.
    /// </summary>
    /// <param name="materiaId">Materia.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>El número de inscritos.</returns>
    public Task<int> ContarInscritosAsync(int materiaId, CancellationToken cancelacion) =>
        Task.FromResult(estudiantes.Estudiantes.Count(estudiante => estudiante.TieneInscrita(materiaId)));

    /// <summary>
    /// Agrega la materia y le asigna el siguiente identificador, como haría la base de datos.
    /// </summary>
    /// <param name="materia">Materia nueva.</param>
    public void Agregar(Materia materia)
    {
        ArgumentNullException.ThrowIfNull(materia);
        materia.Asignar(nameof(Materia.Id), Materias.Count == 0 ? 1 : Materias.Max(actual => actual.Id) + 1);
        Materias.Add(materia);
    }
}
