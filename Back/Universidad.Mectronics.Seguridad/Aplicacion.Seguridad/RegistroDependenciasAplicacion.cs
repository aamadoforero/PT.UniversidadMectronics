using Aplicacion.Seguridad.Servicios.Autenticacion;
using Aplicacion.Seguridad.Servicios.Catalogos;
using Aplicacion.Seguridad.Servicios.CuentaPropia;
using Aplicacion.Seguridad.Servicios.Usuarios;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Aplicacion.Seguridad;

/// <summary>Registro de los servicios de la capa de Aplicación.</summary>
public static class RegistroDependenciasAplicacion
{
    /// <summary>
    /// Registra los casos de uso de la capa de Aplicación. La <c>PoliticaBloqueo</c> la registra la API a partir de su
    /// configuración.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection InyectarDependenciasAplicacion(this IServiceCollection servicios)
    {
        servicios.AddValidatorsFromAssembly(typeof(RegistroDependenciasAplicacion).Assembly, ServiceLifetime.Singleton);

        servicios.AddScoped<GeneradorSesion>();
        servicios.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
        servicios.AddScoped<IServicioCuentaPropia, ServicioCuentaPropia>();

        servicios.AddScoped<AutorizadorUsuarios>();
        servicios.AddScoped<VerificadorDatosAcademicos>();
        servicios.AddScoped<IServicioConsultaUsuarios, ServicioConsultaUsuarios>();
        servicios.AddScoped<IServicioRegistroUsuarios, ServicioRegistroUsuarios>();
        servicios.AddScoped<IServicioAccesoUsuarios, ServicioAccesoUsuarios>();
        servicios.AddScoped<IServicioCatalogos, ServicioCatalogos>();

        return servicios;
    }
}
