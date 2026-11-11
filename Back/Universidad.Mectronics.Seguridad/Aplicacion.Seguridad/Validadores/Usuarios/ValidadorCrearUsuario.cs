using Aplicacion.Seguridad.Dtos.Usuarios;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.Usuarios;

/// <summary>
/// Reglas de formato para crear una cuenta. Qué datos académicos son obligatorios depende del tipo de perfil del rol, que
/// se conoce al consultar la base de datos; por eso esa parte la verifica el servicio.
/// </summary>
public sealed class ValidadorCrearUsuario : AbstractValidator<CrearUsuarioDto>
{
    /// <summary>
    /// Define las reglas de cada campo.
    /// </summary>
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
