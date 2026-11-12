using Api.Logs.Autenticacion;
using Api.Logs.Errores;
using Microsoft.OpenApi;
using Universidad.Mectronics.Transversal.Auditoria;

namespace Api.Logs.Configuracion;

/// <summary>
/// Registro de los servicios de la capa de API: autenticación, errores, controladores y OpenAPI.
/// </summary>
public static class RegistroDependenciasApi
{
    /// <summary>
    /// Registra los servicios de la API y valida su configuración al arrancar.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <param name="configuracion">Configuración de la aplicación.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection ConfiguracionApi(this IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AgregarAutenticacionLogs(configuracion);

        servicios.AddProblemDetails(opciones => opciones.CustomizeProblemDetails = contexto =>
            contexto.ProblemDetails.Extensions["traceId"] = contexto.HttpContext.TraceIdentifier);
        servicios.AddExceptionHandler<ManejadorExcepcionesGlobal>();

        servicios.AddControllers()
            .ConfigureApiBehaviorOptions(opciones => opciones.InvalidModelStateResponseFactory = RespuestaModeloInvalido.Crear);

        servicios.AddOpenApi(opciones => opciones.AddDocumentTransformer((documento, _, _) =>
        {
            documento.Info.Title = "Universidad Mectronics - Servicio de Logs";
            documento.Info.Version = "v1";
            documento.Info.Description =
                "Registro centralizado de eventos de auditoría y errores de los microservicios. Cada petición debe enviar la " +
                "cabecera X-Api-Key con la clave de la aplicación que registra el evento.";

            documento.Components ??= new OpenApiComponents();
            documento.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            documento.Components.SecuritySchemes[EsquemaClaveAplicacion.Nombre] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = NombresCabecera.ClaveAplicacion,
                Description = "Clave de la aplicación que registra el evento."
            };
            documento.Security =
            [
                new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference(EsquemaClaveAplicacion.Nombre, documento)] = [] }
            ];
            return Task.CompletedTask;
        }));

        return servicios;
    }
}
