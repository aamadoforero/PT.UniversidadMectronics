using Aplicacion.Seguridad.Dtos.CuentaPropia;
using Dominio.Seguridad.Politicas;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.CuentaPropia;

/// <summary>
/// Reglas de formato del cambio de contraseña. La política de la contraseña nueva la define el dominio
/// (<see cref="PoliticaContrasena"/>); el validador reporta cada requisito incumplido como un mensaje del campo, para que
/// el usuario vea todo lo que debe corregir de una vez. Verificar la contraseña actual le corresponde al servicio.
/// </summary>
public sealed class ValidadorCambiarContrasena : AbstractValidator<CambiarContrasenaDto>
{
    /// <summary>
    /// Define las reglas de cada campo.
    /// </summary>
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
