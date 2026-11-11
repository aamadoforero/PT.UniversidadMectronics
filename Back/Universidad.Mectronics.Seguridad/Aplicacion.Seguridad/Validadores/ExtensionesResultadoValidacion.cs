using FluentValidation.Results;

namespace Aplicacion.Seguridad.Validadores;

/// <summary>
/// Conversión del resultado de FluentValidation al formato de detalle por campo que usa <c>Error.Validacion</c>, para que
/// el resto de la aplicación no dependa de los tipos de FluentValidation.
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
