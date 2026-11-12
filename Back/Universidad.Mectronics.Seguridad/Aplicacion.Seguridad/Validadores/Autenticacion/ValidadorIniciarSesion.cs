using Aplicacion.Seguridad.Dtos.Autenticacion;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.Autenticacion;

/// <summary>Reglas de formato de los datos de inicio de sesión.</summary>
public sealed class ValidadorIniciarSesion : AbstractValidator<IniciarSesionDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorIniciarSesion"/>.</summary>
    public ValidadorIniciarSesion()
    {
        RuleFor(datos => datos.Email).ValidarCorreo();

        RuleFor(datos => datos.Contrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}
