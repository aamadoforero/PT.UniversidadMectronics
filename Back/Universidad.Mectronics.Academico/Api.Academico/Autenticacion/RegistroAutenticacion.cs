using Aplicacion.Academico.Interfaces;
using Dominio.Academico.Autorizacion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Universidad.Mectronics.Transversal.Autenticacion;
using Universidad.Mectronics.Transversal.Configuracion;

namespace Api.Academico.Autenticacion;

/// <summary>
/// Registro de la autenticación con el token de acceso de Seguridad y de las políticas de autorización por permiso.
/// </summary>
public static class RegistroAutenticacion
{
    /// <summary>Margen de tolerancia entre los relojes de Seguridad y Académico.</summary>
    private static readonly TimeSpan ToleranciaReloj = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Registra JwtBearer con las claves públicas del JWKS de Seguridad, una política por permiso y la política por defecto
    /// que exige sesión. Valida la configuración al arrancar.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <param name="configuracion">Configuración con la sección <c>Jwt</c>.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection AgregarAutenticacionAcademico(this IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AddOptions<OpcionesJwt>()
            .Bind(configuracion.GetSection(OpcionesJwt.Seccion))
            .Validate(opciones => !string.IsNullOrWhiteSpace(opciones.Emisor), MensajesConfiguracion.CrearMensajeFaltante("Jwt:Emisor"))
            .Validate(opciones => !string.IsNullOrWhiteSpace(opciones.Audiencia), MensajesConfiguracion.CrearMensajeFaltante("Jwt:Audiencia"))
            .Validate(opciones => Uri.TryCreate(opciones.UrlClavesPublicas, UriKind.Absolute, out Uri? _),
                MensajesConfiguracion.CrearMensajeInvalido("Jwt:UrlClavesPublicas", "debe ser la URL absoluta del JWKS de Seguridad."))
            .ValidateOnStart();

        servicios.AddHttpContextAccessor();
        servicios.AddScoped<IUsuarioActual, UsuarioActualHttp>();
        servicios.AddScoped<EventosJwtAcademico>();

        servicios.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        servicios.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<OpcionesJwt>>((opciones, jwt) =>
            {
                OpcionesJwt valores = jwt.Value;
                opciones.MapInboundClaims = false;
                opciones.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    valores.UrlClavesPublicas!,
                    new RecuperadorClavesJwks(),
                    new HttpDocumentRetriever { RequireHttps = valores.RequiereHttps });
                opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = valores.Emisor,
                    ValidAudience = valores.Audiencia,
                    ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
                    ValidTypes = [TiposToken.Acceso],
                    NameClaimType = NombresClaim.Nombre,
                    RoleClaimType = NombresClaim.Rol,
                    ClockSkew = ToleranciaReloj
                };
                opciones.EventsType = typeof(EventosJwtAcademico);
            });

        servicios.AddAuthorization(opciones =>
        {
            foreach (string codigo in CodigosPermiso.Todos)
            {
                opciones.AddPolicy(codigo, politica => politica.RequireAuthenticatedUser().RequireClaim(NombresClaim.Permiso, codigo));
            }

            opciones.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });

        return servicios;
    }
}
