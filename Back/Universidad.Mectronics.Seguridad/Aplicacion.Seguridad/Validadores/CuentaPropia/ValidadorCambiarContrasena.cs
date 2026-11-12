using Aplicacion.Seguridad.Dtos.CuentaPropia;
using Dominio.Seguridad.Politicas;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.CuentaPropia;

/// <summary>
/// Reglas de formato del cambio de contraseña. Reporta cada requisito incumplido de <see cref="PoliticaContrasena"/>.
/// </summary>
public sealed class ValidadorCambiarContrasena : AbstractValidator<CambiarContrasenaDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorCambiarContrasena"/>.</summary>
    public ValidadorCambiarContrasena()
    {
        RuleFor(datos => datos.ContrasenaActual)
            .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

        RuleFor(datos => datos.ContrasenaNueva)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("La contraseña nueva es obligatoria.")
            .Custom((contrasena, contexto) =>
            {
                foreach (string incumplimiento in PoliticaContrasena.ObtenerIncumplimientos(contrasena))
                {
                    contexto.AddFailure(nameof(CambiarContrasenaDto.ContrasenaNueva), incumplimiento);
                }
            })
            .NotEqual(datos => datos.ContrasenaActual).WithMessage("La contraseña nueva debe ser distinta de la actual.");
    }
}
