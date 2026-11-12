using Api.Academico.ContrasenaTemporal;
using Api.Academico.Csrf;

namespace Api.Academico.Configuracion;

/// <summary>Configura el pipeline HTTP del servicio.</summary>
public static class ConfiguracionPipeline
{
    /// <summary>
    /// Configura los middlewares y registra los controladores.
    /// </summary>
    /// <param name="aplicacion">Aplicación web.</param>
    /// <returns>La misma aplicación, para encadenar configuraciones.</returns>
    public static WebApplication UsarApiAcademico(this WebApplication aplicacion)
    {
        aplicacion.UseExceptionHandler();

        if (aplicacion.Environment.IsDevelopment())
        {
            aplicacion.UseSwaggerUI(opciones =>
            {
                opciones.SwaggerEndpoint("/openapi/v1.json", "Académico v1");
                opciones.DocumentTitle = "Universidad Mectronics - Académico";
                opciones.UseRequestInterceptor(
                    "(solicitud) => { const m = document.cookie.match(/(?:^|; )universidad-csrf=([^;]*)/); " +
                    "if (m) { solicitud.headers['X-CSRF-TOKEN'] = decodeURIComponent(m[1]); } return solicitud; }");
            });
        }
        else
        {
            aplicacion.UseHsts();
            aplicacion.UseHttpsRedirection();
        }

        aplicacion.UseCors(RegistroDependenciasApi.PoliticaCors);
        aplicacion.UseMiddleware<MiddlewareCsrf>();
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
