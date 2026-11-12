using Aplicacion.Academico.Dtos.Materias;
using FluentValidation;

namespace Aplicacion.Academico.Validadores.Materias;

/// <summary>
/// Reglas de formato de la edición de una materia. Todos los campos son obligatorios: la edición reemplaza los valores.
/// </summary>
public sealed class ValidadorActualizarMateria : AbstractValidator<ActualizarMateriaDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorActualizarMateria"/>.</summary>
    public ValidadorActualizarMateria()
    {
        RuleFor(datos => datos.Nombre).Cascade(CascadeMode.Stop).ValidarNombreMateria();
        RuleFor(datos => datos.ProfesorId).Cascade(CascadeMode.Stop).ValidarIdentificadorObligatorio("El profesor");
    }
}
