using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Errores;

/// <summary>
/// Errores esperados del inicio y la renovación de sesión.
/// </summary>
public static class ErroresAutenticacion
{
    /// <summary>
    /// Credenciales rechazadas. Se usa el mismo error para correo inexistente, contraseña incorrecta, cuenta inactiva y
    /// cuenta bloqueada, para no revelar a un atacante qué correos están registrados ni el estado de una cuenta.
    /// </summary>
    /// <returns>El error de tipo no autorizado.</returns>
    public static Error CrearCredencialesInvalidas() => Error.CrearNoAutorizado(
        "Autenticacion.CredencialesInvalidas",
        "El correo o la contraseña no son correctos.");

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
