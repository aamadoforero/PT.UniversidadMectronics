using Aplicacion.Seguridad.Interfaces;
using Infraestructura.Seguridad.Configuracion;
using Infraestructura.Seguridad.Criptografia;
using Infraestructura.Seguridad.Persistencia;
using Infraestructura.Seguridad.Persistencia.Repositorios;
using Infraestructura.Seguridad.Tiempo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Universidad.Mectronics.Transversal.Configuracion;
using Universidad.Mectronics.Transversal.Comunicacion;

namespace Infraestructura.Seguridad;

/// <summary>
/// Registro de las dependencias de Infraestructura: persistencia, criptografía, reloj y comunicación con otros microservicios.
/// </summary>
public static class RegistroDependenciasInfraestructura
{
    /// <summary>Nombre de la cadena de conexión en <c>ConnectionStrings</c>.</summary>
    public const string NombreCadenaConexion = "UniversidadMectronics";

    /// <summary>
    /// Registra las implementaciones de las interfaces de Aplicación y valida la configuración al arrancar.
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
        servicios.AddScoped<IUnidadTrabajo, UnidadTrabajo>();
        servicios.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
        servicios.AddScoped<IRepositorioRoles, RepositorioRoles>();
        servicios.AddScoped<IRepositorioInscripciones, RepositorioInscripciones>();
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
        servicios.InyectarClienteMicroservicios(configuracion);

        return servicios;
    }
}
