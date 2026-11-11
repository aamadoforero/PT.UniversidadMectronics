using Aplicacion.Seguridad.Servicios.Autenticacion;
using Aplicacion.Seguridad.Servicios.Catalogos;
using Aplicacion.Seguridad.Servicios.CuentaPropia;
using Aplicacion.Seguridad.Servicios.Usuarios;
using Dominio.Seguridad.Politicas;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Aplicacion.Seguridad;

/// <summary>
/// Registro en el contenedor de inyección de dependencias de los servicios de la capa de Aplicación. La API lo invoca al
/// arrancar; las interfaces de <c>Interfaces/</c> las registra cada capa que las implementa.
/// </summary>
public static class RegistroDependenciasAplicacion
{
    /// <summary>
    /// Registra los casos de uso y las políticas de la capa de Aplicación.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection InyectarDependenciasAplicacion(this IServiceCollection servicios)
    {
        servicios.AddSingleton(PoliticaBloqueo.Predeterminada);

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
