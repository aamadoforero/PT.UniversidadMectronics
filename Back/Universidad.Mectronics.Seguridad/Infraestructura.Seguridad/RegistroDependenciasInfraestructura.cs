using Aplicacion.Seguridad.Interfaces;
using Infraestructura.Seguridad.Auditoria;
using Infraestructura.Seguridad.Configuracion;
using Infraestructura.Seguridad.Criptografia;
using Infraestructura.Seguridad.Persistencia;
using Infraestructura.Seguridad.Persistencia.Repositorios;
using Infraestructura.Seguridad.Tiempo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infraestructura.Seguridad;

/// <summary>
/// Registro en el contenedor de inyección de dependencias de las implementaciones de Infraestructura: persistencia,
/// criptografía, auditoría y reloj. La API lo invoca al arrancar, junto con el registro de la capa de Aplicación.
/// </summary>
public static class RegistroDependenciasInfraestructura
{
    /// <summary>Nombre de la cadena de conexión en <c>ConnectionStrings</c>.</summary>
    public const string NombreCadenaConexion = "UniversidadMectronics";

    /// <summary>
    /// Registra las implementaciones de las interfaces de Aplicación y valida la configuración. Falla al arrancar si falta
    /// la cadena de conexión o algún secreto de JWT, con un mensaje que indica cómo configurarlo.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <param name="configuracion">Configuración (appsettings, user-secrets, variables de entorno).</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection InyectarDependenciasInfraestructura(this IServiceCollection servicios, IConfiguration configuracion)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        string? cadenaConexion = configuracion.GetConnectionString(NombreCadenaConexion);
        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new InvalidOperationException(
                MensajesConfiguracion.CrearMensajeFaltante($"ConnectionStrings:{NombreCadenaConexion}"));
        }

        servicios.AddDbContext<ContextoSeguridad>(opciones =>
            opciones.UseSqlServer(cadenaConexion, sql => sql.EnableRetryOnFailure()));
        servicios.AddSingleton(new OpcionesBaseDatos(cadenaConexion));
        servicios.AddScoped<IUnidadTrabajo, UnidadTrabajo>();
        servicios.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
        servicios.AddScoped<IRepositorioRoles, RepositorioRoles>();
        servicios.AddScoped<IConsultaCatalogos, ConsultaCatalogos>();

        servicios.AddSingleton<IValidateOptions<OpcionesJwt>, ValidadorOpcionesJwt>();
        servicios.AddOptions<OpcionesJwt>()
            .Bind(configuracion.GetSection(OpcionesJwt.Seccion))
            .ValidateOnStart();

        servicios.AddSingleton<ProveedorClavesJwt>();
        servicios.AddSingleton<IServicioTokens, ServicioTokens>();
        servicios.AddSingleton<IServicioHashContrasena, ServicioHashContrasena>();
        servicios.AddSingleton<IGeneradorContrasenaTemporal, GeneradorContrasenaTemporal>();
        servicios.AddSingleton<IReloj, RelojSistema>();
        servicios.AddSingleton<IRegistroEventos, RegistroEventos>();

        return servicios;
    }
}
