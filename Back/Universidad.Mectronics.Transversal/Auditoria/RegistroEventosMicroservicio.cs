using Microsoft.Extensions.Logging;
using Universidad.Mectronics.Transversal.Comunicacion;

namespace Universidad.Mectronics.Transversal.Auditoria;

/// <summary>
/// Implementación de <see cref="IRegistroEventos"/> que escribe el evento en el log de la aplicación y lo envía al
/// microservicio de Logs con <see cref="IClienteMicroservicios"/>, sin esperar la respuesta.
/// </summary>
internal sealed class RegistroEventosMicroservicio : IRegistroEventos
{
    /// <summary>Nombre del microservicio de Logs en la sección <c>Microservicios</c>.</summary>
    public const string NombreServicio = "Logs";

    /// <summary>Clave de configuración con la que este microservicio se identifica ante el de Logs.</summary>
    public const string ClaveConfiguracion = "Logs:ClaveAplicacion";

    /// <summary>Cliente para llamar a otros microservicios.</summary>
    private readonly IClienteMicroservicios _cliente;

    /// <summary>Reloj del sistema, para fechar el evento en el momento en que ocurre.</summary>
    private readonly TimeProvider _reloj;

    /// <summary>Log de la aplicación (consola).</summary>
    private readonly ILogger<RegistroEventosMicroservicio> _registrador;

    /// <summary>Cabecera con la clave de la aplicación que se envía en cada evento.</summary>
    private readonly IReadOnlyDictionary<string, string> _cabeceras;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="RegistroEventosMicroservicio"/>.</summary>
    /// <param name="cliente">Cliente para llamar a otros microservicios.</param>
    /// <param name="claveAplicacion">Clave con la que este microservicio se identifica ante el de Logs.</param>
    /// <param name="reloj">Reloj del sistema.</param>
    /// <param name="registrador">Log de la aplicación.</param>
    public RegistroEventosMicroservicio(
        IClienteMicroservicios cliente,
        string claveAplicacion,
        TimeProvider reloj,
        ILogger<RegistroEventosMicroservicio> registrador)
    {
        _cliente = cliente;
        _reloj = reloj;
        _registrador = registrador;
        _cabeceras = new Dictionary<string, string> { [NombresCabecera.ClaveAplicacion] = claveAplicacion };
    }

    /// <summary>
    /// Registra un evento informativo.
    /// </summary>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="cancelacion">No se usa: el evento se envía aunque la petición se cancele.</param>
    /// <returns>Una tarea completada; el envío continúa en segundo plano.</returns>
    public Task RegistrarInformacionAsync(string mensaje, CancellationToken cancelacion)
    {
        _registrador.LogInformation("{Mensaje}", mensaje);
        Disparar(TiposEvento.Informativo, mensaje, null);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Registra una advertencia.
    /// </summary>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="cancelacion">No se usa: el evento se envía aunque la petición se cancele.</param>
    /// <returns>Una tarea completada; el envío continúa en segundo plano.</returns>
    public Task RegistrarAdvertenciaAsync(string mensaje, CancellationToken cancelacion)
    {
        _registrador.LogWarning("{Mensaje}", mensaje);
        Disparar(TiposEvento.Advertencia, mensaje, null);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Registra un error con el detalle técnico de la excepción.
    /// </summary>
    /// <param name="mensaje">Descripción del error.</param>
    /// <param name="excepcion">Excepción que lo causó.</param>
    /// <param name="cancelacion">No se usa: el error se envía aunque la petición se cancele.</param>
    /// <returns>Una tarea completada; el envío continúa en segundo plano.</returns>
    public Task RegistrarErrorAsync(string mensaje, Exception excepcion, CancellationToken cancelacion)
    {
        _registrador.LogError(excepcion, "{Mensaje}", mensaje);
        Disparar(TiposEvento.Error, mensaje, excepcion?.ToString());
        return Task.CompletedTask;
    }

    /// <summary>
    /// Fecha el evento e inicia su envío sin esperarlo.
    /// </summary>
    /// <param name="tipo">Nombre del tipo de evento.</param>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="detalleExcepcion">Detalle técnico, o <see langword="null"/> si no aplica.</param>
    private void Disparar(string tipo, string mensaje, string? detalleExcepcion) =>
        _ = EnviarAsync(new RegistrarEventoDto(tipo, mensaje, detalleExcepcion, _reloj.GetUtcNow()));

    /// <summary>
    /// Envía el evento. Atrapa cualquier fallo porque la tarea no se espera.
    /// </summary>
    /// <param name="evento">Evento a enviar.</param>
    /// <returns>Una tarea que termina cuando el evento se envía o se descarta.</returns>
    private async Task EnviarAsync(RegistrarEventoDto evento)
    {
        try
        {
            RespuestaMicroservicio respuesta = await _cliente.EnviarAsync(NombreServicio, RutasLogs.Eventos, evento, _cabeceras);
            if (!respuesta.EsExitosa)
            {
                _registrador.LogError("El microservicio de Logs rechazó el evento ({Codigo}): {Mensaje}",
                    (int)respuesta.CodigoEstado, evento.Mensaje);
            }
        }
        catch (Exception excepcion)
        {
            _registrador.LogError(excepcion, "No se pudo enviar el evento al microservicio de Logs: {Mensaje}", evento.Mensaje);
        }
    }
}
