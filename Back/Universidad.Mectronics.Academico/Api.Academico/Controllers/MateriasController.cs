using Aplicacion.Academico.Dtos.Materias;
using Aplicacion.Academico.Servicios.Materias;
using Dominio.Academico.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Academico.Controllers;

/// <summary>
/// Materias (<c>/api/academico/materias</c>). La consulta de materias activas la usa cualquier sesión; crear, editar y cambiar
/// el estado exigen <c>materias.gestionar</c>.
/// </summary>
/// <param name="servicio">Caso de uso de materias.</param>
[Route("api/academico/materias")]
[Tags("Materias")]
[Authorize]
public sealed class MateriasController(IServicioMaterias servicio) : AcademicoControllerBase
{
    /// <summary>
    /// Obtiene las materias, ordenadas por nombre.
    /// </summary>
    /// <param name="incluirInactivas">Si se incluyen las desactivadas (exige <c>materias.gestionar</c>).</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con las materias, o 403 si pide las inactivas sin permiso.</returns>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<MateriaDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerTodasAsync([FromQuery] bool incluirInactivas, CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerTodasAsync(incluirInactivas, cancelacion));

    /// <summary>
    /// Obtiene una materia.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con la materia, o 404 si no existe.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType<MateriaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorIdAsync(int id, CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerPorIdAsync(id, cancelacion));

    /// <summary>
    /// Crea una materia de 3 créditos.
    /// </summary>
    /// <param name="datos">Nombre, programa y profesor.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>201 con la materia; 400 por datos inválidos o 409 si el nombre se repite o el profesor ya dicta dos materias.</returns>
    [HttpPost]
    [Authorize(Policy = CodigosPermisoMaterias.Gestionar)]
    [ProducesResponseType<MateriaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CrearAsync(CrearMateriaDto datos, CancellationToken cancelacion)
    {
        Resultado<MateriaDto> resultado = await servicio.CrearAsync(datos, cancelacion);
        return resultado.EsExitoso
            ? Created($"/api/academico/materias/{resultado.Valor.Id}", resultado.Valor)
            : ConvertirAProblema(resultado.Error);
    }

    /// <summary>
    /// Edita el nombre y el profesor de una materia.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="datos">Datos nuevos.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con la materia; 400, 404 o 409 si no se puede editar.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Policy = CodigosPermisoMaterias.Gestionar)]
    [ProducesResponseType<MateriaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarAsync(int id, ActualizarMateriaDto datos, CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ActualizarAsync(id, datos, cancelacion));

    /// <summary>
    /// Activa o desactiva una materia.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="datos">Estado nuevo.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con la materia; 404 si no existe o 409 si tiene inscritos o el profesor no tiene cupo.</returns>
    [HttpPut("{id:int}/estado")]
    [Authorize(Policy = CodigosPermisoMaterias.Gestionar)]
    [ProducesResponseType<MateriaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CambiarEstadoAsync(int id, CambiarEstadoMateriaDto datos, CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.CambiarEstadoAsync(id, datos, cancelacion));
}
