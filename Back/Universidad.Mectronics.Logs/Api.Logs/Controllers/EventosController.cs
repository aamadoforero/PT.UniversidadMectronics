using Api.Logs.Errores;
using Aplicacion.Logs.Servicios.Eventos;
using Microsoft.AspNetCore.Mvc;
using Universidad.Mectronics.Transversal.Auditoria;

namespace Api.Logs.Controllers;

/// <summary>
/// Registro de eventos (<c>/api/logs/eventos</c>). Exige la clave de aplicación; la aplicación de origen sale de esa clave.
/// </summary>
/// <param name="servicio">Caso de uso de registro de eventos.</param>
[ApiController]
[Route(RutasLogs.Eventos)]
[Tags("Eventos")]
public sealed class EventosController(IServicioRegistroEventos servicio) : ControllerBase
{
    /// <summary>
    /// Recibe un evento de auditoría o un error. Lo valida y lo deja en la cola; se guarda en la base de datos en segundo plano.
    /// </summary>
    /// <param name="datos">Tipo, mensaje, detalle de la excepción y fecha del evento.</param>
    /// <param name="cancelacion">Cancelación de la petición.</param>
    /// <returns>202 si el evento fue aceptado, 400 con datos inválidos, 401 sin clave válida o 403 si la aplicación no está registrada.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IResult> RegistrarAsync(RegistrarEventoDto datos, CancellationToken cancelacion) =>
        (await servicio.RegistrarAsync(datos, cancelacion)).ConvertirARespuestaAceptada();
}
