namespace Api.Logs.Configuracion;

/// <summary>
/// Orden del pipeline HTTP: primero el manejo de errores (para atrapar todo lo que sigue), Swagger UI en desarrollo, luego
/// autenticación y autorización, y al final los controladores.
/// </summary>
public static class ConfiguracionPipeline
{
    /// <summary>
    /// Configura los middlewares y registra los controladores.
    /// </summary>
    /// <param name="aplicacion">Aplicación web.</param>
    /// <returns>La misma aplicación, para encadenar configuraciones.</returns>
    public static WebApplication UsarApiLogs(this WebApplication aplicacion)
    {
        aplicacion.UseExceptionHandler();

        if (aplicacion.Environment.IsDevelopment())
        {
            aplicacion.UseSwaggerUI(opciones =>
            {
                opciones.SwaggerEndpoint("/openapi/v1.json", "Logs v1");
                opciones.DocumentTitle = "Universidad Mectronics - Logs";
            });
        }
        else
        {
            aplicacion.UseHsts();
            aplicacion.UseHttpsRedirection();
        }

        aplicacion.UseAuthentication();
        aplicacion.UseAuthorization();

        if (aplicacion.Environment.IsDevelopment())
        {
            aplicacion.MapOpenApi().AllowAnonymous();
        }

        aplicacion.MapControllers();
        return aplicacion;
    }
}
