namespace Aplicacion.Seguridad.Dtos.Autenticacion;

/// <summary>
/// Datos que envía el usuario para iniciar sesión.
/// </summary>
/// <param name="Email">Correo de inicio de sesión, tal como lo escribió el usuario.</param>
/// <param name="Contrasena">Contraseña en texto plano.</param>
public sealed record IniciarSesionDto(string? Email, string? Contrasena);
