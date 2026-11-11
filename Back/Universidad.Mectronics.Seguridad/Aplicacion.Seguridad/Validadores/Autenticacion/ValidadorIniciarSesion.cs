using Aplicacion.Seguridad.Dtos.Autenticacion;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.Autenticacion;

/// <summary>
/// Reglas de formato de los datos de inicio de sesión. Solo valida lo que no requiere consultar la base de datos; las
/// credenciales las verifica el servicio. Reutiliza el objeto de valor del dominio para no duplicar la regla del correo.
/// </summary>
public sealed class ValidadorIniciarSesion : AbstractValidator<IniciarSesionDto>
{
    /// <summary>
    /// Define las reglas de cada campo.
    /// </summary>
    public ValidadorIniciarSesion()
    {
        RuleFor(datos => datos.Email).ValidarCorreo();

        RuleFor(datos => datos.Contrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}
