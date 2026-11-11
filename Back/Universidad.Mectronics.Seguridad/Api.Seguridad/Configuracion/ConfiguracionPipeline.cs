using Api.Seguridad.ContrasenaTemporal;
using Api.Seguridad.Csrf;

namespace Api.Seguridad.Configuracion;

/// <summary>
/// Orden del pipeline HTTP. El orden importa: primero el manejo de errores (para atrapar todo lo que sigue), Swagger UI en
/// desarrollo, luego CORS y CSRF, después autenticación y autorización, y al final la regla de contraseña temporal y los controladores.
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
