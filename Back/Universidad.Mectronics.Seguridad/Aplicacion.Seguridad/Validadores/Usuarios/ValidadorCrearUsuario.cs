using Aplicacion.Seguridad.Dtos.Usuarios;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.Usuarios;

/// <summary>Reglas de formato para crear una cuenta.</summary>
public sealed class ValidadorCrearUsuario : AbstractValidator<CrearUsuarioDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorCrearUsuario"/>.</summary>
    public ValidadorCrearUsuario()
    {
        RuleFor(datos => datos.NombreCompleto).ValidarNombreCompleto();
        RuleFor(datos => datos.Identificacion).ValidarIdentificacion();
        RuleFor(datos => datos.Email).ValidarCorreo();

        RuleFor(datos => datos.RolId)
            .NotNull().WithMessage("El rol es obligatorio.");

        RuleFor(datos => datos.UbicacionSemestral).ValidarUbicacionSemestral();
    }
}
