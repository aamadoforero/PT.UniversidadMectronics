using Aplicacion.Seguridad.Dtos.Usuarios;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores.Usuarios;

/// <summary>
/// Reglas de los criterios de búsqueda de cuentas. Limita el tamaño de página para que una consulta no traiga demasiados
/// registros de una vez.
/// </summary>
public sealed class ValidadorBuscarUsuarios : AbstractValidator<BuscarUsuariosDto>
{
    /// <summary>Tamaño máximo de página permitido.</summary>
    public const int TamanoPaginaMaximo = 100;

    /// <summary>
    /// Define las reglas de cada criterio.
    /// </summary>
    public ValidadorBuscarUsuarios()
    {
        RuleFor(datos => datos.NumeroPagina)
            .GreaterThanOrEqualTo(1).WithMessage("El número de página debe ser 1 o mayor.");

        RuleFor(datos => datos.TamanoPagina)
            .InclusiveBetween(1, TamanoPaginaMaximo).WithMessage($"El tamaño de página debe estar entre 1 y {TamanoPaginaMaximo}.");

        RuleFor(datos => datos.Texto)
            .MaximumLength(150).WithMessage("El texto de búsqueda admite máximo 150 caracteres.");
    }
}
