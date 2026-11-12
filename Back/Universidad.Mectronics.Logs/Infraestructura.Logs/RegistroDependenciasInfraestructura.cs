using Aplicacion.Logs.Interfaces;
using Infraestructura.Logs.Cola;
using Infraestructura.Logs.Persistencia;
using Infraestructura.Logs.Persistencia.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Universidad.Mectronics.Transversal.Configuracion;

namespace Infraestructura.Logs;

/// <summary>
/// Registro en el contenedor de inyección de dependencias de las implementaciones de Infraestructura (persistencia, catálogos
/// y la cola de eventos con su proceso de guardado).
/// </summary>
public static class RegistroDependenciasInfraestructura
{
    /// <summary>Nombre de la cadena de conexión en <c>ConnectionStrings</c>.</summary>
    public const string NombreCadenaConexion = "UniversidadMectronics";

    /// <summary>
    /// Registra las implementaciones de las interfaces de Aplicación. Falla al arrancar si falta la cadena de conexión.
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

        servicios.AddDbContext<ContextoLogs>(opciones =>
            opciones.UseSqlServer(cadenaConexion, sql => sql.EnableRetryOnFailure()));
        servicios.AddMemoryCache();
        servicios.AddScoped<IUnidadTrabajo, UnidadTrabajo>();
        servicios.AddScoped<IRepositorioEventos, RepositorioEventos>();
        servicios.AddScoped<IConsultaCatalogos, ConsultaCatalogos>();

        servicios.AddSingleton<ColaEventos>();
        servicios.AddSingleton<IColaEventos>(proveedor => proveedor.GetRequiredService<ColaEventos>());
        servicios.AddHostedService<ProcesadorEventos>();

        return servicios;
    }
}
