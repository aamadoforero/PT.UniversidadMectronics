using Aplicacion.Academico.Dtos.Inscripciones;
using Aplicacion.Academico.Servicios.Inscripciones;
using Dominio.Academico.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Academico.Controllers;

/// <summary>
/// Inscripciones del estudiante autenticado. El estudiante se toma del token, no de la ruta.
/// </summary>
/// <param name="servicio">Caso de uso de inscripciones.</param>
[Route("api/academico/inscripciones/propias")]
[Tags("Inscripciones")]
[Authorize(Policy = CodigosPermisoInscripciones.Propias)]
public sealed class InscripcionesController(IServicioInscripciones servicio) : AcademicoControllerBase
{
    /// <summary>
    /// Obtiene las materias inscritas y los créditos acumulados.
    /// </summary>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con las inscripciones, o 403 si la cuenta no tiene perfil de estudiante.</returns>
    [HttpGet]
    [ProducesResponseType<InscripcionesPropiasDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerPropiasAsync(CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerPropiasAsync(cancelacion));

    /// <summary>
    /// Inscribe una materia: máximo 3 materias (9 créditos) y sin repetir profesor.
    /// </summary>
    /// <param name="datos">Materia a inscribir.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>201 con la inscripción; 400, 404 o 409 con la regla incumplida.</returns>
    [HttpPost]
    [ProducesResponseType<InscripcionDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> InscribirAsync(InscribirMateriaDto datos, CancellationToken cancelacion)
    {
        Resultado<InscripcionDto> resultado = await servicio.InscribirAsync(datos, cancelacion);
        return resultado.EsExitoso
            ? Created("/api/academico/inscripciones/propias", resultado.Valor)
            : ConvertirAProblema(resultado.Error);
    }

    /// <summary>
    /// Cancela la inscripción de una materia.
    /// </summary>
    /// <param name="materiaId">Materia a cancelar.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>204, o 404 si no la tiene inscrita.</returns>
    [HttpDelete("{materiaId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelarAsync(int materiaId, CancellationToken cancelacion)
    {
        Resultado resultado = await servicio.CancelarAsync(materiaId, cancelacion);
        return resultado.EsExitoso ? NoContent() : ConvertirAProblema(resultado.Error);
    }

    /// <summary>
    /// Obtiene los nombres de los compañeros de una clase inscrita.
    /// </summary>
    /// <param name="materiaId">Materia inscrita.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con los nombres, o 403 si no tiene inscrita la materia.</returns>
    [HttpGet("{materiaId:int}/companeros")]
    [ProducesResponseType<CompanerosClaseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerCompanerosAsync(int materiaId, CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerCompanerosAsync(materiaId, cancelacion));
}
