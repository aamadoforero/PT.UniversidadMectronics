namespace Aplicacion.Seguridad.Dtos.Autenticacion;

/// <summary>
/// Datos para renovar una sesión. La API toma el token de la cookie de renovación, que el navegador envía solo.
/// </summary>
/// <param name="TokenRenovacion">Token de renovación recibido en la cookie; <see langword="null"/> si la cookie no llegó.</param>
public sealed record RenovarSesionDto(string? TokenRenovacion);
