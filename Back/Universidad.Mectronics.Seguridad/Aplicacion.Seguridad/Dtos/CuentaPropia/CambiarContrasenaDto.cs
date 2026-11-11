namespace Aplicacion.Seguridad.Dtos.CuentaPropia;

/// <summary>
/// Datos para que el propio usuario cambie su contraseña. Exige la contraseña actual aunque la sesión ya esté iniciada,
/// para que alguien que encuentre una sesión abierta no pueda apropiarse de la cuenta.
/// </summary>
/// <param name="ContrasenaActual">Contraseña vigente, en texto plano.</param>
/// <param name="ContrasenaNueva">Contraseña nueva, que debe cumplir la política de contraseñas.</param>
public sealed record CambiarContrasenaDto(string? ContrasenaActual, string? ContrasenaNueva);
