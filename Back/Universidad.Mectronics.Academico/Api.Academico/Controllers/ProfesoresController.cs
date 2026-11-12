using Aplicacion.Academico.Dtos.Profesores;
using Aplicacion.Academico.Servicios.Profesores;
using Dominio.Academico.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Academico.Controllers;

/// <summary>
/// Profesores para el formulario de materias (<c>/api/academico/profesores</c>). Exige <c>materias.gestionar</c>.
/// </summary>
/// <param name="servicio">Caso de uso de profesores.</param>
[Route("api/academico/profesores")]
[Tags("Profesores")]
[Authorize(Policy = CodigosPermisoMaterias.Gestionar)]
public sealed class ProfesoresController(IServicioProfesores servicio) : AcademicoControllerBase
{
    /// <summary>
    /// Obtiene los profesores y cuántas materias dicta cada uno.
    /// </summary>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con los profesores.</returns>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProfesorDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosAsync(CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerTodosAsync(cancelacion));
}
