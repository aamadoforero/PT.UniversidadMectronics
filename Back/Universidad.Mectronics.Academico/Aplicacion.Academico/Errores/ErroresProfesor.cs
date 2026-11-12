using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Errores;

/// <summary>
/// Errores esperados de la consulta del profesor.
/// </summary>
public static class ErroresProfesor
{
    /// <summary>
    /// Crea el error para una cuenta sin perfil de profesor.
    /// </summary>
    /// <returns>El error de falta de permiso.</returns>
    public static Error CrearSinPerfilProfesor() => Error.CrearProhibido(
        "Profesor.SinPerfilProfesor",
        "Su cuenta no tiene perfil de profesor.");
}
