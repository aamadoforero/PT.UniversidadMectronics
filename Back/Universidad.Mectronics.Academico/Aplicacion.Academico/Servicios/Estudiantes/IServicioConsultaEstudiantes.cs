using Aplicacion.Academico.Dtos.Estudiantes;
using Universidad.Mectronics.Transversal.Paginacion;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Servicios.Estudiantes;

/// <summary>
/// Consulta de los registros de inscripción de los estudiantes (R8).
/// </summary>
public interface IServicioConsultaEstudiantes
{
    /// <summary>
    /// Busca estudiantes y sus materias inscritas, paginados.
    /// </summary>
    /// <param name="criterios">Texto a buscar en el nombre y página solicitada.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La página de estudiantes, o un error si los criterios no son válidos.</returns>
    Task<Resultado<Pagina<EstudianteResumenDto>>> BuscarAsync(BuscarEstudiantesDto criterios, CancellationToken cancelacion);
}
