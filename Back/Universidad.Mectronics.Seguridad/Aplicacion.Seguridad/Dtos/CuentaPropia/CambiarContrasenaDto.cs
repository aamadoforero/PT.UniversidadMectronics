namespace Aplicacion.Seguridad.Dtos.CuentaPropia;

/// <summary>Datos para que el usuario cambie su contraseña.</summary>
/// <param name="ContrasenaActual">Contraseña vigente, en texto plano.</param>
/// <param name="ContrasenaNueva">Contraseña nueva, que debe cumplir la política de contraseñas.</param>
public sealed record CambiarContrasenaDto(string? ContrasenaActual, string? ContrasenaNueva);
