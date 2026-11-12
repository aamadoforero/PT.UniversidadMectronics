using Aplicacion.Academico.Dtos.Profesores;
using Aplicacion.Academico.Servicios.Profesores;
using Dominio.Academico.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Academico.Controllers;

/// <summary>
/// Materias que dicta el profesor autenticado. El profesor se toma del token.
/// </summary>
/// <param name="servicio">Caso de uso de profesores.</param>
[Route("api/academico/materias/dictadas")]
[Tags("Profesores")]
[Authorize(Policy = CodigosPermisoMaterias.Dictadas)]
public sealed class MateriasDictadasController(IServicioProfesores servicio) : AcademicoControllerBase
{
    /// <summary>
    /// Obtiene las materias del profesor con los nombres de sus estudiantes inscritos.
    /// </summary>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con las materias, o 403 si la cuenta no tiene perfil de profesor.</returns>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<MateriaDictadaDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerAsync(CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerMateriasDictadasAsync(cancelacion));
}
