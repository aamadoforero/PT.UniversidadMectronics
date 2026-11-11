namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Datos de una cuenta para mostrarla en un listado.
/// </summary>
/// <param name="Id">Identificador de la cuenta.</param>
/// <param name="NombreCompleto">Nombre completo.</param>
/// <param name="Identificacion">Documento de identidad.</param>
/// <param name="Email">Correo de inicio de sesión.</param>
/// <param name="Rol">Nombre del rol.</param>
/// <param name="Activo">Indica si la cuenta está habilitada.</param>
/// <param name="EstaBloqueado">Indica si la cuenta está bloqueada en este momento por intentos fallidos.</param>
/// <param name="DebeCambiarContrasena">Indica que la contraseña es temporal.</param>
public sealed record UsuarioResumenDto(
    int Id,
    string NombreCompleto,
    string Identificacion,
    string Email,
    string Rol,
    bool Activo,
    bool EstaBloqueado,
    bool DebeCambiarContrasena);
