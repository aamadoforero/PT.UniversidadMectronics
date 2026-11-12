using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace Universidad.Mectronics.Transversal.Resiliencia;

/// <summary>
/// Política de resiliencia del cliente entre microservicios: tiempo máximo total, reintentos con espera exponencial,
/// circuit breaker y tiempo máximo por intento.
/// </summary>
internal static class ExtensionesResiliencia
{
    /// <summary>Nombre de la política en los logs y métricas de Polly.</summary>
    private const string NombrePolitica = "microservicios";

    /// <summary>
    /// Agrega la política al cliente HTTP. Se crea una instancia por destino (host y puerto), así cada microservicio tiene su
    /// propio circuit breaker.
    /// </summary>
    /// <param name="cliente">Constructor del cliente HTTP.</param>
    /// <param name="parametros">Parámetros de la política.</param>
    /// <returns>El mismo constructor, para encadenar configuraciones.</returns>
    public static IHttpClientBuilder AgregarResiliencia(this IHttpClientBuilder cliente, OpcionesResiliencia parametros)
    {
        cliente.AddResilienceHandler(NombrePolitica, (canalizacion, contexto) =>
        {
            ILogger registrador = contexto.ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(ExtensionesResiliencia).FullName!);
            string destino = contexto.InstanceName;

            canalizacion.AddTimeout(parametros.TiempoMaximoTotal);

            canalizacion.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = parametros.MaximoReintentos,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = parametros.EsperaBase,
                ShouldHandle = argumentos => ValueTask.FromResult(EsFalloTransitorio(argumentos.Outcome)),
                OnRetry = argumentos =>
                {
                    registrador.LogWarning("Reintento {Intento} de la llamada a {Destino} en {Espera} ms.",
                        argumentos.AttemptNumber + 1, destino, argumentos.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            });

            canalizacion.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = parametros.ProporcionFallos,
                MinimumThroughput = parametros.MinimoLlamadas,
                SamplingDuration = parametros.VentanaMuestreo,
                BreakDuration = parametros.DuracionCorte,
                ShouldHandle = argumentos => ValueTask.FromResult(EsFalloTransitorio(argumentos.Outcome)),
                OnOpened = argumentos =>
                {
                    registrador.LogError("{Destino} no responde: se suspenden las llamadas por {Segundos} s.",
                        destino, argumentos.BreakDuration.TotalSeconds);
                    return ValueTask.CompletedTask;
                },
                OnClosed = _ =>
                {
                    registrador.LogInformation("{Destino} volvió a responder: se reanudan las llamadas.", destino);
                    return ValueTask.CompletedTask;
                }
            });

            canalizacion.AddTimeout(parametros.TiempoMaximoIntento);
        }).SelectPipelineByAuthority();

        return cliente;
    }

    /// <summary>
    /// Indica si el resultado justifica reintentar: sin conexión, tiempo agotado, 408, 429 o 5xx.
    /// </summary>
    /// <param name="resultado">Respuesta o excepción del intento.</param>
    /// <returns><see langword="true"/> si el fallo puede ser pasajero.</returns>
    private static bool EsFalloTransitorio(Outcome<HttpResponseMessage> resultado) => resultado switch
    {
        { Exception: HttpRequestException or TimeoutRejectedException } => true,
        { Result.StatusCode: HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests } => true,
        { Result: { } respuesta } => (int)respuesta.StatusCode >= 500,
        _ => false
    };
}
