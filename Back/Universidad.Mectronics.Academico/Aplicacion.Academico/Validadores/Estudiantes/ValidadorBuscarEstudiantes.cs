using Aplicacion.Academico.Dtos.Estudiantes;
using FluentValidation;

namespace Aplicacion.Academico.Validadores.Estudiantes;

/// <summary>
/// Reglas de los criterios de búsqueda de estudiantes.
/// </summary>
public sealed class ValidadorBuscarEstudiantes : AbstractValidator<BuscarEstudiantesDto>
{
    /// <summary>Tamaño de página máximo, para que una consulta no traiga la tabla completa.</summary>
    public const int TamanoPaginaMaximo = 100;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorBuscarEstudiantes"/>.</summary>
    public ValidadorBuscarEstudiantes()
    {
        RuleFor(datos => datos.NumeroPagina)
            .GreaterThanOrEqualTo(1).WithMessage("El número de página debe ser 1 o mayor.");

        RuleFor(datos => datos.TamanoPagina)
            .InclusiveBetween(1, TamanoPaginaMaximo).WithMessage($"El tamaño de página debe estar entre 1 y {TamanoPaginaMaximo}.");

        RuleFor(datos => datos.Texto)
            .MaximumLength(150).WithMessage("El texto de búsqueda admite máximo 150 caracteres.");
    }
}
