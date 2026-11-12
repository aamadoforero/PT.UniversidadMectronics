using Aplicacion.Seguridad.Dtos.Catalogos;
using Aplicacion.Seguridad.Servicios.Catalogos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Seguridad.Controllers;

/// <summary>
/// Catálogos para los formularios de administración (<c>/api/seguridad/catalogos</c>). Exigen sesión; el servicio verifica
/// que la cuenta tenga alguno de los permisos de esos formularios.
/// </summary>
/// <param name="servicio">Caso de uso de catálogos.</param>
[Route("api/seguridad/catalogos")]
[Tags("Catálogos")]
[Authorize]
public sealed class CatalogosController(IServicioCatalogos servicio) : SeguridadControllerBase
{
    /// <summary>
    /// Obtiene los roles asignables, con su tipo de perfil.
    /// </summary>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con los roles, o 403 sin permiso.</returns>
    [HttpGet("roles")]
    [ProducesResponseType<IReadOnlyList<RolDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerRolesAsync(CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerRolesAsignablesAsync(cancelacion));

    /// <summary>
    /// Obtiene los programas académicos activos.
    /// </summary>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con los programas, o 403 sin permiso.</returns>
    [HttpGet("programas")]
    [ProducesResponseType<IReadOnlyList<ElementoCatalogoDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerProgramasAsync(CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerProgramasActivosAsync(cancelacion));

    /// <summary>
    /// Obtiene las categorías de profesor.
    /// </summary>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con las categorías, o 403 sin permiso.</returns>
    [HttpGet("categorias-profesor")]
    [ProducesResponseType<IReadOnlyList<ElementoCatalogoDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerCategoriasAsync(CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerCategoriasProfesorAsync(cancelacion));
}
