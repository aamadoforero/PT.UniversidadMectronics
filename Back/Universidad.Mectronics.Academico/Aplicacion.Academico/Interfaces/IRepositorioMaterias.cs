using Dominio.Academico.Entidades;

namespace Aplicacion.Academico.Interfaces;

/// <summary>
/// Acceso a las materias.
/// </summary>
public interface IRepositorioMaterias
{
    /// <summary>
    /// Obtiene una materia con su profesor. Queda en seguimiento para guardar sus cambios.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La materia, o <see langword="null"/> si no existe.</returns>
    Task<Materia?> ObtenerPorIdAsync(int id, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene las materias con su profesor, ordenadas por nombre.
    /// </summary>
    /// <param name="incluirInactivas">Si se incluyen las materias desactivadas.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Las materias.</returns>
    Task<IReadOnlyList<Materia>> ObtenerTodasAsync(bool incluirInactivas, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene las materias activas que dicta un profesor, ordenadas por nombre.
    /// </summary>
    /// <param name="profesorId">Profesor.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Las materias del profesor.</returns>
    Task<IReadOnlyList<Materia>> ObtenerActivasPorProfesorAsync(int profesorId, CancellationToken cancelacion);

    /// <summary>
    /// Cuenta las materias activas de un profesor (R6).
    /// </summary>
    /// <param name="profesorId">Profesor.</param>
    /// <param name="materiaExcluidaId">Materia que no se cuenta (la que se está editando), o <see langword="null"/>.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El número de materias activas.</returns>
    Task<int> ContarActivasPorProfesorAsync(int profesorId, int? materiaExcluidaId, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene cuántas materias activas dicta cada profesor.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El número de materias activas por identificador de profesor; los profesores sin materias no aparecen.</returns>
    Task<IReadOnlyDictionary<int, int>> ObtenerConteoActivasPorProfesorAsync(CancellationToken cancelacion);

    /// <summary>
    /// Indica si el programa ya tiene una materia con ese nombre.
    /// </summary>
    /// <param name="programaId">Programa.</param>
    /// <param name="nombre">Nombre a buscar.</param>
    /// <param name="materiaExcluidaId">Materia que no se compara (la que se está editando), o <see langword="null"/>.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si el nombre ya está en uso.</returns>
    Task<bool> ExisteNombreAsync(int programaId, string nombre, int? materiaExcluidaId, CancellationToken cancelacion);

    /// <summary>
    /// Cuenta los estudiantes con inscripción vigente en una materia.
    /// </summary>
    /// <param name="materiaId">Materia.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El número de inscritos.</returns>
    Task<int> ContarInscritosAsync(int materiaId, CancellationToken cancelacion);

    /// <summary>
    /// Agrega una materia nueva. Se guarda al llamar a <see cref="IUnidadTrabajo.GuardarCambiosAsync"/>.
    /// </summary>
    /// <param name="materia">Materia a crear.</param>
    void Agregar(Materia materia);
}
