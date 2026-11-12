using Aplicacion.Academico.Dtos.Estudiantes;
using Aplicacion.Academico.Servicios.Estudiantes;
using Dominio.Academico.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universidad.Mectronics.Transversal.Paginacion;

namespace Api.Academico.Controllers;

/// <summary>
/// Registros de inscripción de los estudiantes (<c>/api/academico/estudiantes</c>). Exige <c>inscripciones.consultar</c> y
/// solo expone nombre, programa y materias.
/// </summary>
/// <param name="servicio">Caso de uso de consulta de estudiantes.</param>
[Route("api/academico/estudiantes")]
[Tags("Estudiantes")]
[Authorize(Policy = CodigosPermisoInscripciones.Consultar)]
public sealed class EstudiantesController(IServicioConsultaEstudiantes servicio) : AcademicoControllerBase
{
    /// <summary>
    /// Busca estudiantes por nombre, paginados, con las materias que tiene inscritas cada uno.
    /// </summary>
    /// <param name="criterios">Texto, número y tamaño de página.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con la página, o 400 por criterios inválidos.</returns>
    [HttpGet]
    [ProducesResponseType<Pagina<EstudianteResumenDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BuscarAsync([FromQuery] BuscarEstudiantesDto criterios, CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.BuscarAsync(criterios, cancelacion));
}
