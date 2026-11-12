using System.Globalization;
using System.Threading.RateLimiting;
using Api.Gateway.Errores;
using Microsoft.Extensions.Options;

namespace Api.Gateway.Configuracion;

/// <summary>
/// Registro de los servicios del gateway: proxy inverso (YARP), límite de solicitudes, Problem Details y chequeo de
/// salud.
/// </summary>
public static class RegistroDependenciasGateway
{
    /// <summary>Duración de la ventana del límite de solicitudes.</summary>
    private static readonly TimeSpan VentanaLimite = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Registra los servicios del gateway.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <param name="configuracion">Configuración con las secciones <c>ReverseProxy</c> y <c>LimiteSolicitudes</c>.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection ConfiguracionGateway(this IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AddProblemDetails(opciones => opciones.CustomizeProblemDetails = contexto =>
            contexto.ProblemDetails.Extensions["traceId"] = contexto.HttpContext.TraceIdentifier);

        servicios.AddReverseProxy().LoadFromConfig(configuracion.GetSection("ReverseProxy"));

        servicios.AddOptions<OpcionesLimiteSolicitudes>()
            .Bind(configuracion.GetSection(OpcionesLimiteSolicitudes.Seccion))
            .Validate(opciones => opciones.PermitidasPorMinuto > 0, "La configuración 'LimiteSolicitudes:PermitidasPorMinuto' debe ser mayor que cero.")
            .ValidateOnStart();

        servicios.AddRateLimiter(opciones =>
        {
            opciones.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(contexto =>
            {
                int permitidas = contexto.RequestServices.GetRequiredService<IOptions<OpcionesLimiteSolicitudes>>().Value.PermitidasPorMinuto;
                return RateLimitPartition.GetFixedWindowLimiter(
                    contexto.Connection.RemoteIpAddress?.ToString() ?? "desconocida",
                    _ => new FixedWindowRateLimiterOptions { PermitLimit = permitidas, Window = VentanaLimite, QueueLimit = 0 });
            });
            opciones.OnRejected = async (contexto, cancelacion) =>
            {
                contexto.HttpContext.Response.Headers.RetryAfter = ((int)VentanaLimite.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                await RespuestasError.CrearDemasiadasSolicitudes().ExecuteAsync(contexto.HttpContext);
            };
        });

        servicios.AddHealthChecks();
        return servicios;
    }
}
