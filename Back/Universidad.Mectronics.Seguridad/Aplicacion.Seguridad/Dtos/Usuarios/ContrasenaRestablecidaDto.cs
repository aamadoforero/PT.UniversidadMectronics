namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Contraseña temporal generada al restablecer el acceso de una cuenta. Solo se entrega en esta respuesta.
/// </summary>
/// <param name="UsuarioId">Cuenta restablecida.</param>
/// <param name="ContrasenaTemporal">Contraseña temporal en texto plano, que el usuario deberá cambiar al iniciar sesión.</param>
public sealed record ContrasenaRestablecidaDto(int UsuarioId, string ContrasenaTemporal);
