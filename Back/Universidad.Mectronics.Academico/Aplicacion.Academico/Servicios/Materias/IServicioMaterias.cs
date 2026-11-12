using Aplicacion.Academico.Dtos.Materias;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Servicios.Materias;

/// <summary>
/// Consulta y administración de materias (CRUD). La consulta de materias activas la usa cualquier sesión; la administración
/// exige el permiso <c>materias.gestionar</c>, que verifica la API.
/// </summary>
public interface IServicioMaterias
{
    /// <summary>
    /// Obtiene las materias, ordenadas por nombre.
    /// </summary>
    /// <param name="incluirInactivas">Si se incluyen las desactivadas; exige <c>materias.gestionar</c>.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Las materias, o un error de falta de permiso.</returns>
    Task<Resultado<IReadOnlyList<MateriaDto>>> ObtenerTodasAsync(bool incluirInactivas, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene una materia. Las desactivadas solo las ve quien las administra.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La materia, o un error si no existe.</returns>
    Task<Resultado<MateriaDto>> ObtenerPorIdAsync(int id, CancellationToken cancelacion);

    /// <summary>
    /// Crea una materia de 3 créditos (R4), con máximo dos materias por profesor (R6).
    /// </summary>
    /// <param name="datos">Datos de la materia.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La materia creada, o el error de la regla incumplida.</returns>
    Task<Resultado<MateriaDto>> CrearAsync(CrearMateriaDto datos, CancellationToken cancelacion);

    /// <summary>
    /// Edita el nombre y el profesor de una materia.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="datos">Datos nuevos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La materia editada, o el error de la regla incumplida.</returns>
    Task<Resultado<MateriaDto>> ActualizarAsync(int id, ActualizarMateriaDto datos, CancellationToken cancelacion);

    /// <summary>
    /// Activa o desactiva una materia.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="datos">Estado nuevo.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La materia con su estado nuevo, o el error de la regla incumplida.</returns>
    Task<Resultado<MateriaDto>> CambiarEstadoAsync(int id, CambiarEstadoMateriaDto datos, CancellationToken cancelacion);
}
