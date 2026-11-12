using Aplicacion.Seguridad.Dtos.Usuarios;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.Usuarios;

/// <summary>Reglas de formato para editar una cuenta.</summary>
public sealed class ValidadorActualizarUsuario : AbstractValidator<ActualizarUsuarioDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorActualizarUsuario"/>.</summary>
    public ValidadorActualizarUsuario()
    {
        RuleFor(datos => datos.NombreCompleto).ValidarNombreCompleto();
        RuleFor(datos => datos.Identificacion).ValidarIdentificacion();
        RuleFor(datos => datos.Email).ValidarCorreo();
        RuleFor(datos => datos.UbicacionSemestral).ValidarUbicacionSemestral();
    }
}
