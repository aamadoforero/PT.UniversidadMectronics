namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Cuenta creada y su contraseña temporal, que solo se entrega en esta respuesta.
/// </summary>
/// <param name="Usuario">Datos de la cuenta creada.</param>
/// <param name="ContrasenaTemporal">Contraseña temporal en texto plano, que el usuario deberá cambiar al iniciar
/// sesión.</param>
public sealed record UsuarioCreadoDto(UsuarioDetalleDto Usuario, string ContrasenaTemporal);
