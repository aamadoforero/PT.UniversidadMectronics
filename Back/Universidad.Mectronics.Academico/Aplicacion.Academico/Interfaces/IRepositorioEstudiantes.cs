using Aplicacion.Academico.Dtos.Estudiantes;
using Dominio.Academico.Entidades;
using Universidad.Mectronics.Transversal.Paginacion;

namespace Aplicacion.Academico.Interfaces;

/// <summary>
/// Acceso a los estudiantes y sus inscripciones vigentes.
/// </summary>
public interface IRepositorioEstudiantes
{
    /// <summary>
    /// Obtiene el estudiante de una cuenta, con su cuenta, su programa y sus inscripciones vigentes (cada una con su materia).
    /// Queda en seguimiento para guardar los cambios de sus inscripciones.
    /// </summary>
    /// <param name="usuarioId">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El estudiante, o <see langword="null"/> si la cuenta no tiene perfil de estudiante.</returns>
    Task<Estudiante?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancelacion);

    /// <summary>
    /// Busca estudiantes con cuenta activa, ordenados por nombre y paginados, con sus inscripciones vigentes.
    /// </summary>
    /// <param name="filtro">Texto a buscar en el nombre y página solicitada.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La página de estudiantes.</returns>
    Task<Pagina<Estudiante>> BuscarAsync(BuscarEstudiantesDto filtro, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene los nombres de los estudiantes con inscripción vigente en una materia, ordenados alfabéticamente.
    /// </summary>
    /// <param name="materiaId">Materia.</param>
    /// <param name="estudianteExcluidoId">Estudiante que se omite (quien consulta), o 0 para no omitir ninguno.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Los nombres de los compañeros.</returns>
    Task<IReadOnlyList<string>> ObtenerNombresInscritosAsync(int materiaId, int estudianteExcluidoId, CancellationToken cancelacion);
}
