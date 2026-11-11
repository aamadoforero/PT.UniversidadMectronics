using Aplicacion.Seguridad.Dtos.Usuarios;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.Usuarios;

/// <summary>
/// Reglas para cambiar el rol de una cuenta.
/// </summary>
public sealed class ValidadorCambiarRol : AbstractValidator<CambiarRolDto>
{
    /// <summary>
    /// Define las reglas de cada campo.
    /// </summary>
    public ValidadorCambiarRol()
    {
        RuleFor(datos => datos.RolId)
            .NotNull().WithMessage("El rol es obligatorio.");
    }
}
