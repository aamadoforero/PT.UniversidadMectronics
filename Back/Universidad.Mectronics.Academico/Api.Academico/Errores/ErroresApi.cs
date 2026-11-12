using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Academico.Errores;

/// <summary>
/// Errores generados por la API antes de llegar a un caso de uso: autenticación, autorización, CSRF y contraseña
/// temporal.
/// </summary>
public static class ErroresApi
{
    /// <summary>
    /// Crea el error para una petición sin sesión válida (sin cookie, con un token vencido o con la cuenta desactivada).
    /// </summary>
    /// <returns>El error de tipo no autorizado (401).</returns>
    public static Error CrearNoAutenticado() => Error.CrearNoAutorizado(
        "Autenticacion.NoAutenticado",
        "Debe iniciar sesión para continuar.");

    /// <summary>
    /// Crea el error para una sesión válida sin el permiso que exige el endpoint.
    /// </summary>
    /// <returns>El error de falta de permiso (403).</returns>
    public static Error CrearSinPermiso() => Error.CrearProhibido(
        "Autorizacion.SinPermiso",
        "No tiene permiso para realizar esta operación.");

    /// <summary>
    /// Crea el error para una petición que modifica datos sin el token contra CSRF correcto.
    /// </summary>
    /// <returns>El error de falta de permiso (403).</returns>
    public static Error CrearCsrfInvalido() => Error.CrearProhibido(
        "Seguridad.CsrfInvalido",
        "La solicitud no incluye un token de seguridad válido. Recargue la página e intente de nuevo.");

    /// <summary>
    /// Crea el error para una sesión con contraseña temporal: en Académico no puede hacer nada hasta cambiarla en Seguridad.
    /// </summary>
    /// <returns>El error de falta de permiso (403).</returns>
    public static Error CrearCambioContrasenaObligatorio() => Error.CrearProhibido(
        "Autenticacion.CambioContrasenaObligatorio",
        "Debe cambiar su contraseña temporal antes de continuar.");
}
