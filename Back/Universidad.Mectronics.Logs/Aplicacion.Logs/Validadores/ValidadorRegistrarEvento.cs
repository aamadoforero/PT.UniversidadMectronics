using Dominio.Logs.Entidades;
using FluentValidation;
using Universidad.Mectronics.Transversal.Auditoria;

namespace Aplicacion.Logs.Validadores;

/// <summary>
/// Reglas de formato del evento recibido. Que el tipo exista lo verifica el servicio.
/// </summary>
public sealed class ValidadorRegistrarEvento : AbstractValidator<RegistrarEventoDto>
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="ValidadorRegistrarEvento"/>.</summary>
    public ValidadorRegistrarEvento()
    {
        RuleFor(datos => datos.Tipo)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El tipo de evento es obligatorio.")
            .MaximumLength(TipoEvento.LongitudMaximaNombre)
            .WithMessage($"El tipo de evento admite máximo {TipoEvento.LongitudMaximaNombre} caracteres.");

        RuleFor(datos => datos.Mensaje)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El mensaje es obligatorio.")
            .MaximumLength(RegistroEvento.LongitudMaximaMensaje)
            .WithMessage($"El mensaje admite máximo {RegistroEvento.LongitudMaximaMensaje} caracteres.");

        RuleFor(datos => datos.Excepcion)
            .MaximumLength(RegistroEvento.LongitudMaximaExcepcion)
            .WithMessage($"El detalle de la excepción admite máximo {RegistroEvento.LongitudMaximaExcepcion} caracteres.");

        RuleFor(datos => datos.Fecha)
            .NotNull().WithMessage("La fecha del evento es obligatoria.");
    }
}
