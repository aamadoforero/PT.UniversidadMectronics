using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Autorizacion;
using Infraestructura.Seguridad.Criptografia;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Seguridad.Autenticacion;

/// <summary>
/// Registra la autenticación con JWT y la autorización por permisos.
/// </summary>
public static class RegistroAutenticacion
{
    /// <summary>
    /// Configura JWT con la clave pública de <see cref="ProveedorClavesJwt"/>, una política de autorización por cada permiso
    /// del servicio y la exigencia de sesión por defecto en todos los endpoints.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection AgregarAutenticacionSeguridad(this IServiceCollection servicios)
    {
        servicios.AddHttpContextAccessor();
        servicios.AddScoped<IUsuarioActual, UsuarioActualHttp>();
        servicios.AddScoped<EventosJwtSeguridad>();

        servicios.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        servicios.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<ProveedorClavesJwt>((opciones, proveedorClaves) =>
            {
                opciones.MapInboundClaims = false;
                opciones.TokenValidationParameters = proveedorClaves.CrearParametrosValidacionAcceso();
                opciones.EventsType = typeof(EventosJwtSeguridad);
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
