using Aplicacion.Academico.Dtos.Materias;
using FluentValidation;

namespace Aplicacion.Academico.Validadores.Materias;

/// <summary>Reglas de formato para crear una materia.</summary>
public sealed class ValidadorCrearMateria : AbstractValidator<CrearMateriaDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorCrearMateria"/>.</summary>
    public ValidadorCrearMateria()
    {
        RuleFor(datos => datos.Nombre).Cascade(CascadeMode.Stop).ValidarNombreMateria();
        RuleFor(datos => datos.ProgramaId).Cascade(CascadeMode.Stop).ValidarIdentificadorObligatorio("El programa");
        RuleFor(datos => datos.ProfesorId).Cascade(CascadeMode.Stop).ValidarIdentificadorObligatorio("El profesor");
    }
}
