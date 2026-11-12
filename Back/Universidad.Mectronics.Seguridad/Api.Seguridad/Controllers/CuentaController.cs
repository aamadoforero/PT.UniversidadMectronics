using Api.Seguridad.ContrasenaTemporal;
using Api.Seguridad.Dtos;
using Api.Seguridad.Sesion;
using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Dtos.CuentaPropia;
using Aplicacion.Seguridad.Servicios.CuentaPropia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Seguridad.Controllers;

/// <summary>Cuenta del usuario autenticado: perfil y cambio de contraseña.</summary>
/// <param name="servicio">Caso de uso de la propia cuenta.</param>
/// <param name="cookies">Gestor de las cookies de sesión.</param>
[Route("api/seguridad/cuenta")]
[Tags("Cuenta propia")]
[Authorize]
public sealed class CuentaController(IServicioCuentaPropia servicio, GestorCookiesSesion cookies) : SeguridadControllerBase
{
    /// <summary>
    /// Obtiene el perfil completo de la cuenta de la sesión. Se permite aunque la contraseña sea temporal.
    /// </summary>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con el perfil, o 401 si la cuenta ya no está activa.</returns>
    [HttpGet("perfil")]
    [PermiteContrasenaTemporal]
    [ProducesResponseType<PerfilDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObtenerPerfilAsync(CancellationToken cancelacion) =>
        ConvertirARespuesta(await servicio.ObtenerPerfilAsync(cancelacion));

    /// <summary>
    /// Cambia la contraseña verificando la actual. Las sesiones anteriores quedan invalidadas y se escriben cookies nuevas.
    /// Se permite aunque la contraseña sea temporal.
    /// </summary>
    /// <param name="datos">Contraseña actual y nueva.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>200 con los datos de la sesión nueva, o 400 si la contraseña actual o la nueva no son válidas.</returns>
    [HttpPut("contrasena")]
    [PermiteContrasenaTemporal]
    [ProducesResponseType<RespuestaSesionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CambiarContrasenaAsync(CambiarContrasenaDto datos, CancellationToken cancelacion)
    {
        Resultado<SesionCreadaDto> resultado = await servicio.CambiarContrasenaAsync(datos, cancelacion);
        return resultado.EsExitoso
            ? Ok(cookies.EscribirSesion(Response, resultado.Valor))
            : ConvertirAProblema(resultado.Error);
    }
}
