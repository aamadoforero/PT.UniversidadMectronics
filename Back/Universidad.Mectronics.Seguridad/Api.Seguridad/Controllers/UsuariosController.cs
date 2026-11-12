using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Servicios.Usuarios;
using Dominio.Seguridad.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universidad.Mectronics.Transversal.Paginacion;
using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Seguridad.Controllers;

/// <summary>Administración de cuentas. Cada acción exige su permiso.</summary>
/// <param name="consulta">Caso de uso de consulta de cuentas.</param>
/// <param name="registro">Caso de uso de creación y edición de cuentas.</param>
/// <param name="acceso">Caso de uso de rol, estado y contraseña de las cuentas.</param>
[Route("api/seguridad/usuarios")]
[Tags("Usuarios")]
public sealed class UsuariosController(
    IServicioConsultaUsuarios consulta,
    IServicioRegistroUsuarios registro,
    IServicioAccesoUsuarios acceso) : SeguridadControllerBase
{
    /// <summary>
    /// Busca cuentas con filtros opcionales y paginación, ordenadas por nombre.
    /// </summary>
    /// <param name="criterios">Filtros por query string: texto, rolId, activo, numeroPagina (desde 1) y tamanoPagina (1 a 100).</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con la página de cuentas, o 400 por criterios inválidos.</returns>
    [HttpGet]
    [Authorize(Policy = CodigosPermisoUsuarios.Consultar)]
    [ProducesResponseType<Pagina<UsuarioResumenDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BuscarAsync([FromQuery] BuscarUsuariosDto criterios, CancellationToken cancelacion) =>
        ConvertirARespuesta(await consulta.BuscarAsync(criterios, cancelacion));

    /// <summary>
    /// Obtiene el detalle de una cuenta.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con el detalle, o 404 si no existe.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Policy = CodigosPermisoUsuarios.Consultar)]
    [ProducesResponseType<UsuarioDetalleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerAsync(int id, CancellationToken cancelacion) =>
        ConvertirARespuesta(await consulta.ObtenerAsync(id, cancelacion));

    /// <summary>
    /// Crea una cuenta con su perfil académico y una contraseña temporal, que solo se entrega en esta respuesta.
    /// </summary>
    /// <param name="datos">Datos de la cuenta y, según el rol, sus datos académicos.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>201 con la cuenta y su contraseña temporal; 400, 403 o 409 si no se puede crear.</returns>
    [HttpPost]
    [Authorize(Policy = CodigosPermisoUsuarios.Crear)]
    [ProducesResponseType<UsuarioCreadoDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CrearAsync(CrearUsuarioDto datos, CancellationToken cancelacion)
    {
        Resultado<UsuarioCreadoDto> resultado = await registro.CrearAsync(datos, cancelacion);
        return resultado.EsExitoso
            ? Created($"/api/seguridad/usuarios/{resultado.Valor.Usuario.Id}", resultado.Valor)
            : ConvertirAProblema(resultado.Error);
    }

    /// <summary>
    /// Edita los datos de una cuenta y su perfil académico. El rol tiene su propia acción.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="datos">Datos nuevos.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con la cuenta actualizada; 400, 404 o 409 si no se puede editar.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Policy = CodigosPermisoUsuarios.Editar)]
    [ProducesResponseType<UsuarioDetalleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarAsync(int id, ActualizarUsuarioDto datos, CancellationToken cancelacion) =>
        ConvertirARespuesta(await registro.ActualizarAsync(id, datos, cancelacion));

    /// <summary>
    /// Activa o desactiva una cuenta. Una cuenta desactivada pierde el acceso en la petición siguiente; si es un
    /// estudiante, se cancelan sus inscripciones vigentes.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="datos">Estado nuevo.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con la cuenta actualizada; 400, 404 o 409 si no se puede cambiar.</returns>
    [HttpPut("{id:int}/estado")]
    [Authorize(Policy = CodigosPermisoUsuarios.CambiarEstado)]
    [ProducesResponseType<UsuarioDetalleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CambiarEstadoAsync(int id, CambiarEstadoDto datos, CancellationToken cancelacion) =>
        ConvertirARespuesta(await acceso.CambiarEstadoAsync(id, datos, cancelacion));

    /// <summary>
    /// Genera una contraseña temporal para una cuenta, la desbloquea e invalida sus sesiones.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con la contraseña temporal; 404 o 409 si no se puede restablecer.</returns>
    [HttpPost("{id:int}/contrasena-temporal")]
    [Authorize(Policy = CodigosPermisoUsuarios.RestablecerContrasena)]
    [ProducesResponseType<ContrasenaRestablecidaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RestablecerContrasenaAsync(int id, CancellationToken cancelacion) =>
        ConvertirARespuesta(await acceso.RestablecerContrasenaAsync(id, cancelacion));
}
