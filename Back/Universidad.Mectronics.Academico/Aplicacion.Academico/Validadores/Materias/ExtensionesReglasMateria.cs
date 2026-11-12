using Dominio.Academico.Entidades;
using FluentValidation;

namespace Aplicacion.Academico.Validadores.Materias;

/// <summary>
/// Reglas de campo compartidas por los validadores de materias, con los límites que define el dominio.
/// </summary>
public static class ExtensionesReglasMateria
{
    /// <summary>
    /// Exige un nombre no vacío dentro de la longitud de <see cref="Materia.LongitudMaximaNombre"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del DTO validado.</typeparam>
    /// <param name="regla">Regla del campo.</param>
    /// <returns>La misma regla, para encadenar.</returns>
    public static IRuleBuilderOptions<T, string?> ValidarNombreMateria<T>(this IRuleBuilder<T, string?> regla) => regla
        .NotEmpty().WithMessage("El nombre es obligatorio.")
        .MaximumLength(Materia.LongitudMaximaNombre).WithMessage($"El nombre admite máximo {Materia.LongitudMaximaNombre} caracteres.");

    /// <summary>
    /// Exige un identificador presente y positivo.
    /// </summary>
    /// <typeparam name="T">Tipo del DTO validado.</typeparam>
    /// <param name="regla">Regla del campo.</param>
    /// <param name="nombreCampo">Nombre del dato para el mensaje, por ejemplo <c>El profesor</c>.</param>
    /// <returns>La misma regla, para encadenar.</returns>
    public static IRuleBuilderOptions<T, int?> ValidarIdentificadorObligatorio<T>(this IRuleBuilder<T, int?> regla, string nombreCampo) => regla
        .NotNull().WithMessage($"{nombreCampo} es obligatorio.")
        .GreaterThan(0).WithMessage($"{nombreCampo} no es válido.");
}
