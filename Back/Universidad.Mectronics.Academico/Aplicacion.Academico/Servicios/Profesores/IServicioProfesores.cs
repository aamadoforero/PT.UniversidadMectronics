using Aplicacion.Academico.Dtos.Profesores;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Servicios.Profesores;

/// <summary>
/// Consulta de profesores: el listado para el formulario de materias y las clases del profesor autenticado.
/// </summary>
public interface IServicioProfesores
{
    /// <summary>
    /// Obtiene los profesores con cuenta activa y cuántas materias dicta cada uno.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Los profesores, ordenados por nombre.</returns>
    Task<Resultado<IReadOnlyList<ProfesorDto>>> ObtenerTodosAsync(CancellationToken cancelacion);

    /// <summary>
    /// Obtiene las materias que dicta el profesor autenticado, cada una con los nombres de sus estudiantes inscritos.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Las materias dictadas, o un error si la cuenta no tiene perfil de profesor.</returns>
    Task<Resultado<IReadOnlyList<MateriaDictadaDto>>> ObtenerMateriasDictadasAsync(CancellationToken cancelacion);
}
