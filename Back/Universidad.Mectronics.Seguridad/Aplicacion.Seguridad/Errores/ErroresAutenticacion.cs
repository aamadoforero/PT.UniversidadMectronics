using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Errores;

/// <summary>
/// Errores esperados del inicio y la renovación de sesión.
/// </summary>
public static class ErroresAutenticacion
{
    /// <summary>
    /// Credenciales rechazadas. Es el mismo error para correo inexistente, contraseña incorrecta y cuenta inactiva.
    /// </summary>
    /// <returns>El error de tipo no autorizado.</returns>
    public static Error CrearCredencialesInvalidas() => Error.CrearNoAutorizado(
        "Autenticacion.CredencialesInvalidas",
        "El correo o la contraseña no son correctos.");

    /// <summary>
    /// Cuenta bloqueada temporalmente por superar el máximo de intentos fallidos. Se responde con HTTP 429 mientras el
    /// bloqueo esté vigente, aunque la contraseña sea correcta.
    /// </summary>
    /// <param name="duracionBloqueo">Duración del bloqueo según la política, para indicar cuánto esperar.</param>
    /// <returns>El error de tipo demasiados intentos.</returns>
    public static Error CrearCuentaBloqueada(TimeSpan duracionBloqueo)
    {
        int minutos = (int)Math.Ceiling(duracionBloqueo.TotalMinutes);
        string espera = minutos == 1 ? "1 minuto" : $"{minutos} minutos";
        return Error.CrearDemasiadosIntentos(
            "Autenticacion.CuentaBloqueada",
            $"Demasiados intentos de inicio de sesión. Si se equivocó varias veces, espere {espera} antes de intentar de nuevo.");
    }

    /// <summary>
    /// Token de renovación ausente, vencido, alterado, de una cuenta inactiva o emitido antes de un cambio de contraseña.
    /// </summary>
    /// <returns>El error de tipo no autorizado.</returns>
    public static Error CrearSesionInvalida() => Error.CrearNoAutorizado(
        "Autenticacion.SesionInvalida",
        "La sesión expiró o ya no es válida. Inicie sesión nuevamente.");

    /// <summary>
    /// Faltan datos o tienen un formato inválido.
    /// </summary>
    /// <param name="detalles">Campos inválidos y sus mensajes.</param>
    /// <returns>El error de validación.</returns>
    public static Error CrearDatosInvalidos(IReadOnlyDictionary<string, string[]> detalles) => Error.CrearValidacion(
        "Autenticacion.DatosInvalidos",
        "Revise los datos ingresados.",
        detalles);
}
