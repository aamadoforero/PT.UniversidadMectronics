using Api.Gateway.Errores;

namespace Api.Gateway.Configuracion;

/// <summary>
/// Configura el pipeline del gateway: límite de solicitudes, chequeo de salud y proxy.
/// </summary>
public static class ConfiguracionPipeline
{
    /// <summary>
    /// Configura los middlewares y las rutas del gateway.
    /// </summary>
    /// <param name="aplicacion">Aplicación web.</param>
    /// <returns>La misma aplicación, para encadenar configuraciones.</returns>
    public static WebApplication UsarGateway(this WebApplication aplicacion)
    {
        aplicacion.UseExceptionHandler();
        aplicacion.UseStatusCodePages();
        aplicacion.UseRateLimiter();

        aplicacion.MapHealthChecks("/salud").DisableRateLimiting();
        aplicacion.MapReverseProxy(proxy =>
        {
            proxy.UseMiddleware<MiddlewareErroresProxy>();
            proxy.UseSessionAffinity();
            proxy.UseLoadBalancing();
            proxy.UsePassiveHealthChecks();
        });

        return aplicacion;
    }
}
