using Aplicacion.Seguridad.Dtos.CuentaPropia;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.CuentaPropia;

/// <summary>
/// Reglas de formato de los datos personales que el usuario edita desde su perfil. Reutiliza el objeto de valor del
/// dominio para no duplicar la regla del nombre.
/// </summary>
public sealed class ValidadorActualizarDatosPersonales : AbstractValidator<ActualizarDatosPersonalesDto>
{
    /// <summary>
    /// Define las reglas de cada campo.
    /// </summary>
    public ValidadorActualizarDatosPersonales()
    {
        RuleFor(datos => datos.NombreCompleto).ValidarNombreCompleto();
    }
}
