using System.Text.Json.Serialization;
using Api.Academico.Autenticacion;
using Api.Academico.Errores;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Academico.Configuracion;

/// <summary>
/// Registro de los servicios propios de la capa de API: autenticación, errores, controladores, CORS y OpenAPI.
/// </summary>
public static class RegistroDependenciasApi
{
    /// <summary>Nombre de la política CORS para el frontend.</summary>
    public const string PoliticaCors = "frontend";

    /// <summary>
    /// Registra los servicios de la API y valida su configuración al arrancar.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <param name="configuracion">Configuración de la aplicación.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection ConfiguracionApi(this IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AgregarAutenticacionAcademico(configuracion);

        servicios.AddProblemDetails(opciones => opciones.CustomizeProblemDetails = contexto =>
            contexto.ProblemDetails.Extensions["traceId"] = contexto.HttpContext.TraceIdentifier);
        servicios.AddExceptionHandler<ManejadorExcepcionesGlobal>();

        servicios.AddControllers()
            .AddJsonOptions(opciones => opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .ConfigureApiBehaviorOptions(opciones => opciones.InvalidModelStateResponseFactory = RespuestaModeloInvalido.Crear);

        string[] origenes = configuracion.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];
        servicios.AddCors(opciones => opciones.AddPolicy(PoliticaCors, politica => politica
            .WithOrigins(origenes)
            .AllowCredentials()
            .AllowAnyMethod()
            .WithHeaders("Content-Type", NombresCookie.CabeceraCsrf)));

        servicios.AddOpenApi(opciones => opciones.AddDocumentTransformer((documento, _, _) =>
        {
            documento.Info.Title = "Universidad Mectronics - Servicio Académico";
            documento.Info.Version = "v1";
            documento.Info.Description =
                "Materias e inscripciones. Usa la sesión de Seguridad: inicie sesión en http://localhost:5090 y las cookies se " +
                "envían también a este servicio. Las peticiones que modifican datos deben enviar la cabecera X-CSRF-TOKEN con " +
                "el valor de la cookie universidad-csrf.";
            return Task.CompletedTask;
        }));

        return servicios;
    }
}
