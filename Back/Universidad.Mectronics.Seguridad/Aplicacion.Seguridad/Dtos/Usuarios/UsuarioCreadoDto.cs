namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Cuenta recién creada junto con su contraseña temporal. La contraseña solo se entrega en esta respuesta: no se guarda en
/// texto plano en ningún lugar, así que el administrador debe comunicarla al usuario.
/// </summary>
/// <param name="Usuario">Datos de la cuenta creada.</param>
/// <param name="ContrasenaTemporal">Contraseña temporal en texto plano, que el usuario deberá cambiar al iniciar sesión.</param>
public sealed record UsuarioCreadoDto(UsuarioDetalleDto Usuario, string ContrasenaTemporal);
