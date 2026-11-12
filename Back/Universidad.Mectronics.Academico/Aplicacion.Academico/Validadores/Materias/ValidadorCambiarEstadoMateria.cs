using Aplicacion.Academico.Dtos.Materias;
using FluentValidation;

namespace Aplicacion.Academico.Validadores.Materias;

/// <summary>
/// Reglas de formato del cambio de estado de una materia.
/// </summary>
public sealed class ValidadorCambiarEstadoMateria : AbstractValidator<CambiarEstadoMateriaDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorCambiarEstadoMateria"/>.</summary>
    public ValidadorCambiarEstadoMateria()
    {
        RuleFor(datos => datos.Activo)
            .NotNull().WithMessage("El estado es obligatorio.");
    }
}
