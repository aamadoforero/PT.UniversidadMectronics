using Aplicacion.Academico.Interfaces;
using Infraestructura.Academico.Persistencia;
using Infraestructura.Academico.Persistencia.Repositorios;
using Infraestructura.Academico.Tiempo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Universidad.Mectronics.Transversal.Configuracion;
using Universidad.Mectronics.Transversal.Comunicacion;

namespace Infraestructura.Academico;

/// <summary>
/// Registro de las dependencias de Infraestructura: persistencia, reloj y comunicación con otros microservicios.
/// </summary>
public static class RegistroDependenciasInfraestructura
{
    /// <summary>Nombre de la cadena de conexión en <c>ConnectionStrings</c>.</summary>
    public const string NombreCadenaConexion = "UniversidadMectronics";

    /// <summary>
    /// Registra las implementaciones de las interfaces de Aplicación. Falla al arrancar si falta la cadena de conexión o la
    /// configuración del microservicio de Logs.
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

        servicios.AddDbContext<ContextoAcademico>(opciones =>
            opciones.UseSqlServer(cadenaConexion, sql => sql.EnableRetryOnFailure()));
        servicios.AddScoped<IUnidadTrabajo, UnidadTrabajo>();
        servicios.AddScoped<IRepositorioEstudiantes, RepositorioEstudiantes>();
        servicios.AddScoped<IRepositorioMaterias, RepositorioMaterias>();
        servicios.AddScoped<IRepositorioProfesores, RepositorioProfesores>();
        servicios.AddScoped<IRepositorioProgramas, RepositorioProgramas>();
        servicios.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
        servicios.AddSingleton<IReloj, RelojSistema>();

        servicios.InyectarClienteMicroservicios(configuracion);

        return servicios;
    }
}
