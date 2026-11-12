namespace Aplicacion.Logs.Interfaces;

/// <summary>
/// Aplicación que hace la petición, identificada por su clave. La implementa la API a partir de la autenticación.
/// </summary>
public interface IAplicacionActual
{
    /// <summary>Nombre de la aplicación autenticada (como en <c>Logs.Aplicacion</c>), o <see langword="null"/> si no hay ninguna.</summary>
    string? Nombre { get; }
}
