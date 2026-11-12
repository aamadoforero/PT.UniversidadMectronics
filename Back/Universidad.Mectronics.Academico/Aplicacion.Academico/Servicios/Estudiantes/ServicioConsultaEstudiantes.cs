using Aplicacion.Academico.Dtos.Estudiantes;
using Aplicacion.Academico.Errores;
using Aplicacion.Academico.Interfaces;
using Aplicacion.Academico.Mapeos;
using Aplicacion.Academico.Validadores;
using Dominio.Academico.Entidades;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Paginacion;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Servicios.Estudiantes;

/// <summary>
/// Implementación de la consulta de estudiantes. Solo expone nombre, programa y materias: ningún dato personal.
/// </summary>
/// <param name="repositorioEstudiantes">Acceso a los estudiantes.</param>
/// <param name="validador">Reglas de los criterios de búsqueda.</param>
public sealed class ServicioConsultaEstudiantes(
    IRepositorioEstudiantes repositorioEstudiantes,
    IValidator<BuscarEstudiantesDto> validador) : IServicioConsultaEstudiantes
{
    /// <summary>
    /// Valida los criterios, normaliza el texto y consulta la página.
    /// </summary>
    /// <param name="criterios">Texto a buscar en el nombre y página solicitada.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La página de estudiantes, o un error si los criterios no son válidos.</returns>
    public async Task<Resultado<Pagina<EstudianteResumenDto>>> BuscarAsync(BuscarEstudiantesDto criterios, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(criterios);

        ValidationResult validacion = await validador.ValidateAsync(criterios, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresEstudiante.CrearCriteriosInvalidos(validacion.ObtenerDetalleError());
        }

        string? texto = string.IsNullOrWhiteSpace(criterios.Texto) ? null : criterios.Texto.Trim();
        Pagina<Estudiante> pagina = await repositorioEstudiantes.BuscarAsync(criterios with { Texto = texto }, cancelacion);
        return pagina.Convertir(estudiante => estudiante.ConvertirAEstudianteResumenDto());
    }
}
