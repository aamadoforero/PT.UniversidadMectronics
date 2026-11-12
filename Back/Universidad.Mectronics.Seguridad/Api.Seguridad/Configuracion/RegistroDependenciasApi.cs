using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Api.Seguridad.Autenticacion;
using Api.Seguridad.Errores;
using Api.Seguridad.Sesion;
using Dominio.Seguridad.Politicas;
using Microsoft.Extensions.Options;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Seguridad.Configuracion;

/// <summary>
/// Registro de los servicios propios de la capa de API: controladores, autenticación, errores, cookies, CORS, límite de
/// intentos y OpenAPI.
/// </summary>
public static class RegistroDependenciasApi
{
    /// <summary>Nombre de la política que limita los intentos de inicio de sesión.</summary>
    public const string PoliticaInicioSesion = "inicio-sesion";

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
        servicios.AgregarAutenticacionSeguridad();

        servicios.AddProblemDetails(opciones => opciones.CustomizeProblemDetails = contexto =>
            contexto.ProblemDetails.Extensions["traceId"] = contexto.HttpContext.TraceIdentifier);
        servicios.AddExceptionHandler<ManejadorExcepcionesGlobal>();

        servicios.AddControllers()
            .AddJsonOptions(opciones => opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .ConfigureApiBehaviorOptions(opciones => opciones.InvalidModelStateResponseFactory = RespuestaModeloInvalido.Crear);

        servicios.AddOptions<OpcionesSesion>()
            .Bind(configuracion.GetSection(OpcionesSesion.Seccion))
            .Validate(opciones => opciones.MismoSitio != SameSiteMode.None || opciones.CookieSegura,
                "La configuración 'Sesion:MismoSitio' = None exige 'Sesion:CookieSegura' = true.")
            .ValidateOnStart();
        servicios.AddSingleton<GestorCookiesSesion>();

        servicios.AddOptions<OpcionesBloqueo>()
            .Bind(configuracion.GetSection(OpcionesBloqueo.Seccion))
            .Validate(opciones => opciones.IntentosMaximos >= 1, "La configuración 'Bloqueo:IntentosMaximos' debe ser al menos 1.")
            .Validate(opciones => opciones.MinutosBloqueo >= 1, "La configuración 'Bloqueo:MinutosBloqueo' debe ser al menos 1.")
            .ValidateOnStart();
        servicios.AddSingleton(proveedor =>
        {
            OpcionesBloqueo opciones = proveedor.GetRequiredService<IOptions<OpcionesBloqueo>>().Value;
            return new PoliticaBloqueo(opciones.IntentosMaximos, TimeSpan.FromMinutes(opciones.MinutosBloqueo));
        });

        string[] origenes = configuracion.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];
        servicios.AddCors(opciones => opciones.AddPolicy(PoliticaCors, politica => politica
            .WithOrigins(origenes)
            .AllowCredentials()
            .AllowAnyMethod()
            .WithHeaders("Content-Type", NombresCookie.CabeceraCsrf)));

        servicios.AddRateLimiter(opciones =>
        {
            opciones.AddPolicy(PoliticaInicioSesion, contexto => RateLimitPartition.GetFixedWindowLimiter(
                contexto.Connection.RemoteIpAddress?.ToString() ?? "desconocida",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
            opciones.OnRejected = async (contexto, cancelacion) =>
            {
                contexto.HttpContext.Response.Headers.RetryAfter = "60";
                await TypedResults.Problem(
                    title: "Se hicieron demasiados intentos de inicio de sesión desde este equipo. Espere un minuto e intente de nuevo.",
                    statusCode: StatusCodes.Status429TooManyRequests,
                    extensions: new Dictionary<string, object?> { ["codigo"] = "Autenticacion.DemasiadosIntentosEquipo" })
                    .ExecuteAsync(contexto.HttpContext);
            };
        });

        servicios.AddOpenApi(opciones => opciones.AddDocumentTransformer((documento, _, _) =>
        {
            documento.Info.Title = "Universidad Mectronics - Servicio de Seguridad";
            documento.Info.Version = "v1";
            documento.Info.Description =
                "Autenticación, sesión y administración de cuentas. La sesión viaja en cookies HttpOnly; las peticiones que " +
                "modifican datos deben enviar la cabecera X-CSRF-TOKEN con el valor de la cookie universidad-csrf.";
            return Task.CompletedTask;
        }));

        return servicios;
    }
}
