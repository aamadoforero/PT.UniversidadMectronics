using Aplicacion.Logs.Interfaces;
using Dominio.Logs.Entidades;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infraestructura.Logs.Cola;

/// <summary>
/// Proceso en segundo plano que guarda en la base de datos los eventos de la cola. Si un guardado falla, lo registra en
/// el log y continúa.
/// </summary>
/// <param name="cola">Cola de eventos pendientes.</param>
/// <param name="fabricaAmbitos">Crea un ámbito por evento, porque el contexto de Entity Framework no debe vivir toda la
/// aplicación.</param>
/// <param name="registrador">Log de la aplicación.</param>
internal sealed class ProcesadorEventos(ColaEventos cola, IServiceScopeFactory fabricaAmbitos, ILogger<ProcesadorEventos> registrador) : BackgroundService
{
    /// <summary>
    /// Cierra la cola para que el ciclo termine cuando quede vacía, y espera a que termine.
    /// </summary>
    /// <param name="cancellationToken">Indica que se agotó el tiempo de apagado del servicio.</param>
    /// <returns>Una tarea que termina cuando el procesamiento se detiene.</returns>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        cola.Cerrar();
        return base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Guarda los eventos a medida que llegan, hasta que la cola se cierre y quede vacía.
    /// </summary>
    /// <param name="stoppingToken">No se usa: los pendientes se guardan aunque el servicio se esté deteniendo.</param>
    /// <returns>Una tarea que termina cuando la cola se vacía tras cerrarse.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (RegistroEvento evento in cola.Lector.ReadAllAsync(CancellationToken.None))
        {
            await GuardarAsync(evento);
        }
    }

    /// <summary>
    /// Guarda un evento en su propio ámbito de inyección de dependencias.
    /// </summary>
    /// <param name="evento">Evento a guardar.</param>
    /// <returns>Una tarea que termina cuando el evento se guarda o se descarta.</returns>
    private async Task GuardarAsync(RegistroEvento evento)
    {
        try
        {
            await using AsyncServiceScope ambito = fabricaAmbitos.CreateAsyncScope();
            ambito.ServiceProvider.GetRequiredService<IRepositorioEventos>().Agregar(evento);
            await ambito.ServiceProvider.GetRequiredService<IUnidadTrabajo>().GuardarCambiosAsync(CancellationToken.None);
        }
        catch (Exception excepcion)
        {
            registrador.LogError(excepcion, "No se pudo guardar el evento en Logs.RegistroEvento: {Mensaje}", evento.Mensaje);
        }
    }
}
