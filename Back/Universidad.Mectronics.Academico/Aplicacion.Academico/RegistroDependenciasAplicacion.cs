using Aplicacion.Academico.Servicios.Estudiantes;
using Aplicacion.Academico.Servicios.Inscripciones;
using Aplicacion.Academico.Servicios.Materias;
using Aplicacion.Academico.Servicios.Profesores;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Aplicacion.Academico;

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

        servicios.AddScoped<IServicioInscripciones, ServicioInscripciones>();
        servicios.AddScoped<IServicioConsultaEstudiantes, ServicioConsultaEstudiantes>();
        servicios.AddScoped<IServicioMaterias, ServicioMaterias>();
        servicios.AddScoped<IServicioProfesores, ServicioProfesores>();

        return servicios;
    }
}
