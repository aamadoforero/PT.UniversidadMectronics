using FluentValidation.Results;

namespace Aplicacion.Seguridad.Validadores;

/// <summary>
/// Convierte el resultado de FluentValidation en el detalle por campo que usa <c>Error.Validacion</c>.
/// </summary>
public static class ExtensionesResultadoValidacion
{
    /// <summary>
    /// Agrupa los errores por campo, conservando el orden en que se definieron las reglas.
    /// </summary>
    /// <param name="resultado">Resultado de ejecutar un validador.</param>
    /// <returns>Los mensajes de error por nombre de campo.</returns>
    public static IReadOnlyDictionary<string, string[]> ObtenerDetalleError(this ValidationResult resultado) =>
        resultado.Errors
            .GroupBy(error => error.PropertyName, StringComparer.Ordinal)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.Select(error => error.ErrorMessage).ToArray(),
                StringComparer.Ordinal);
}
