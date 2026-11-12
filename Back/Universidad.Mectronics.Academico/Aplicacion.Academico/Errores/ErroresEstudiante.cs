using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Errores;

/// <summary>
/// Errores esperados de la consulta de estudiantes.
/// </summary>
public static class ErroresEstudiante
{
    /// <summary>
    /// Crea el error para criterios de búsqueda inválidos.
    /// </summary>
    /// <param name="detalles">Mensajes de error por campo.</param>
    /// <returns>El error de validación.</returns>
    public static Error CrearCriteriosInvalidos(IReadOnlyDictionary<string, string[]> detalles) => Error.CrearValidacion(
        "Estudiante.CriteriosInvalidos",
        "Los criterios de búsqueda no son válidos.",
        detalles);
}
