using Aplicacion.Seguridad.Dtos.Usuarios;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.Usuarios;

/// <summary>
/// Reglas de formato para editar una cuenta. Qué datos académicos son obligatorios depende del tipo de perfil del rol actual
/// de la cuenta; por eso esa parte la verifica el servicio.
/// </summary>
public sealed class ValidadorActualizarUsuario : AbstractValidator<ActualizarUsuarioDto>
{
    /// <summary>
    /// Define las reglas de cada campo.
    /// </summary>
    public ValidadorActualizarUsuario()
    {
        RuleFor(datos => datos.NombreCompleto).ValidarNombreCompleto();
        RuleFor(datos => datos.Identificacion).ValidarIdentificacion();
        RuleFor(datos => datos.Email).ValidarCorreo();
        RuleFor(datos => datos.UbicacionSemestral).ValidarUbicacionSemestral();
    }
}
