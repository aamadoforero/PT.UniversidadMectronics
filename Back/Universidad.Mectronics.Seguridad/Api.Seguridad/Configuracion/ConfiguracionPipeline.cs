using Api.Seguridad.ContrasenaTemporal;
using Api.Seguridad.Csrf;
using Microsoft.AspNetCore.HttpOverrides;

namespace Api.Seguridad.Configuracion;

/// <summary>
/// Configura el pipeline HTTP del servicio. Las cabeceras <c>X-Forwarded-*</c> del gateway se procesan primero para que
/// el límite de intentos use la IP del cliente.
/// </summary>
public static class ConfiguracionPipeline
{
    /// <summary>
    /// Configura los middlewares y registra los controladores.
    /// </summary>
    /// <param name="aplicacion">Aplicación web.</param>
    /// <returns>La misma aplicación, para encadenar configuraciones.</returns>
    public static WebApplication UsarApiSeguridad(this WebApplication aplicacion)
    {
        aplicacion.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        aplicacion.UseExceptionHandler();

        if (aplicacion.Environment.IsDevelopment())
        {
            aplicacion.UseSwaggerUI(opciones =>
            {
                opciones.SwaggerEndpoint("/openapi/v1.json", "Seguridad v1");
                opciones.DocumentTitle = "Universidad Mectronics - Seguridad";
                opciones.UseRequestInterceptor(
                    "(solicitud) => { const m = document.cookie.match(/(?:^|; )universidad-csrf=([^;]*)/); " +
                    "if (m) { solicitud.headers['X-CSRF-TOKEN'] = decodeURIComponent(m[1]); } return solicitud; }");
            });
        }

        if (!aplicacion.Environment.IsDevelopment())
        {
            aplicacion.UseHsts();
            aplicacion.UseHttpsRedirection();
        }

        aplicacion.UseCors(RegistroDependenciasApi.PoliticaCors);
        aplicacion.UseMiddleware<MiddlewareCsrf>();
        aplicacion.UseRateLimiter();
        aplicacion.UseAuthentication();
        aplicacion.UseAuthorization();
        aplicacion.UseMiddleware<MiddlewareContrasenaTemporal>();

        if (aplicacion.Environment.IsDevelopment())
        {
            aplicacion.MapOpenApi().AllowAnonymous();
        }

        aplicacion.MapControllers();
        return aplicacion;
    }
}
