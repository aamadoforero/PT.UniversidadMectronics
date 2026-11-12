using Aplicacion.Academico.Dtos.Inscripciones;
using FluentValidation;

namespace Aplicacion.Academico.Validadores.Inscripciones;

/// <summary>
/// Reglas de formato de la inscripción. Las reglas de negocio las aplica el servicio con <c>PoliticaInscripcion</c>.
/// </summary>
public sealed class ValidadorInscribirMateria : AbstractValidator<InscribirMateriaDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorInscribirMateria"/>.</summary>
    public ValidadorInscribirMateria()
    {
        RuleFor(datos => datos.MateriaId)
            .NotNull().WithMessage("La materia es obligatoria.")
            .GreaterThan(0).WithMessage("La materia no es válida.");
    }
}
