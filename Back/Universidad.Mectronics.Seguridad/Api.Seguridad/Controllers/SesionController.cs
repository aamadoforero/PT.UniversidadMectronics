using Api.Seguridad.Configuracion;
using Api.Seguridad.ContrasenaTemporal;
using Api.Seguridad.Dtos;
using Api.Seguridad.Csrf;
using Api.Seguridad.Sesion;
using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Servicios.Autenticacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Universidad.Mectronics.Transversal.Autenticacion;
using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Seguridad.Controllers;

/// <summary>
/// Inicio, renovación y cierre de sesión. Los tokens viajan en cookies <c>HttpOnly</c>.
/// </summary>
/// <param name="servicio">Caso de uso de autenticación.</param>
/// <param name="cookies">Gestor de las cookies de sesión.</param>
[Route("api/seguridad/sesion")]
[Tags("Sesión")]
[AllowAnonymous]
[PermiteContrasenaTemporal]
public sealed class SesionController(IServicioAutenticacion servicio, GestorCookiesSesion cookies) : SeguridadControllerBase
{
    /// <summary>
    /// Inicia sesión con correo y contraseña y escribe las cookies de acceso, renovación y CSRF.
    /// </summary>
    /// <param name="datos">Correo y contraseña.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con los datos de la cuenta, 400 por datos inválidos, 401 por credenciales inválidas o 429 por
    /// exceso de intentos.</returns>
    [HttpPost]
    [OmitirValidacionCsrf]
    [EnableRateLimiting(RegistroDependenciasApi.PoliticaInicioSesion)]
    [ProducesResponseType<RespuestaSesionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> IniciarSesionAsync(IniciarSesionDto datos, CancellationToken cancelacion)
    {
        Resultado<SesionCreadaDto> resultado = await servicio.IniciarSesionAsync(datos, cancelacion);
        return resultado.EsExitoso
            ? Ok(cookies.EscribirSesion(Response, resultado.Valor))
            : ConvertirAProblema(resultado.Error);
    }

    /// <summary>
    /// Renueva la sesión con la cookie de renovación y emite tokens nuevos. Si no es posible, elimina las cookies.
    /// </summary>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con los datos de la cuenta, o 401 si la sesión ya no es válida.</returns>
    [HttpPost("renovacion")]
    [ProducesResponseType<RespuestaSesionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RenovarSesionAsync(CancellationToken cancelacion)
    {
        string? token = Request.Cookies[NombresCookie.Renovacion];
        Resultado<SesionCreadaDto> resultado = await servicio.RenovarSesionAsync(new RenovarSesionDto(token), cancelacion);
        if (resultado.EsFallido)
        {
            cookies.EliminarSesion(Response);
            return ConvertirAProblema(resultado.Error);
        }

        return Ok(cookies.EscribirSesion(Response, resultado.Valor));
    }

    /// <summary>Cierra la sesión eliminando las cookies.</summary>
    /// <returns>204 sin contenido.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult CerrarSesion()
    {
        cookies.EliminarSesion(Response);
        return NoContent();
    }
}
