namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Datos para cambiar el rol de una cuenta. Solo se permite hacia un rol con el mismo tipo de perfil.
/// </summary>
/// <param name="RolId">Rol nuevo.</param>
public sealed record CambiarRolDto(byte? RolId);
