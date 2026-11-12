using Aplicacion.Seguridad.Dtos.Usuarios;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.Usuarios;

/// <summary>
/// Reglas para activar o desactivar una cuenta. Exige el estado explícito para no desactivar una cuenta por un valor omitido.
/// </summary>
public sealed class ValidadorCambiarEstado : AbstractValidator<CambiarEstadoDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorCambiarEstado"/>.</summary>
    public ValidadorCambiarEstado()
    {
        RuleFor(datos => datos.Activo)
            .NotNull().WithMessage("Debe indicar si la cuenta queda activa o inactiva.");
    }
}
