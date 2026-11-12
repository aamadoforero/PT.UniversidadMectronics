using Aplicacion.Logs.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Api.Logs.Autenticacion;

/// <summary>
/// Registro de la autenticación por clave de aplicación y de la política que la exige en todos los endpoints.
/// </summary>
public static class RegistroAutenticacion
{
    /// <summary>
    /// Registra el esquema, la validación de las claves al arrancar y la política por defecto.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <param name="configuracion">Configuración con la sección <c>ClavesAplicacion</c>.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection AgregarAutenticacionLogs(this IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AddSingleton<IValidateOptions<OpcionesClavesAplicacion>, ValidadorOpcionesClavesAplicacion>();
        servicios.AddOptions<OpcionesClavesAplicacion>()
            .Bind(configuracion.GetSection(OpcionesClavesAplicacion.Seccion))
            .ValidateOnStart();

        servicios.AddAuthentication(EsquemaClaveAplicacion.Nombre)
            .AddScheme<AuthenticationSchemeOptions, ManejadorClaveAplicacion>(EsquemaClaveAplicacion.Nombre, null);
        servicios.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        servicios.AddHttpContextAccessor();
        servicios.AddScoped<IAplicacionActual, AplicacionActualHttp>();
        return servicios;
    }
}
