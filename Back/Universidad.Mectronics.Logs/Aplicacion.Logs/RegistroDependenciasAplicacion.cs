using Aplicacion.Logs.Servicios.Eventos;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Aplicacion.Logs;

/// <summary>
/// Registro en el contenedor de inyección de dependencias de los servicios de la capa de Aplicación. Las interfaces de
/// <c>Interfaces/</c> las registra cada capa que las implementa.
/// </summary>
public static class RegistroDependenciasAplicacion
{
    /// <summary>
    /// Registra los casos de uso y los validadores.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection InyectarDependenciasAplicacion(this IServiceCollection servicios)
    {
        servicios.AddValidatorsFromAssembly(typeof(RegistroDependenciasAplicacion).Assembly, ServiceLifetime.Singleton);
        servicios.AddScoped<IServicioRegistroEventos, ServicioRegistroEventos>();
        return servicios;
    }
}
