namespace Aplicacion.Seguridad.Dtos.Autenticacion;

/// <summary>
/// Datos de la cuenta autenticada que necesita el frontend para mostrar su nombre y decidir qué menús y acciones habilitar.
/// </summary>
/// <param name="Id">Identificador de la cuenta.</param>
/// <param name="NombreCompleto">Nombre completo.</param>
/// <param name="Email">Correo de inicio de sesión.</param>
/// <param name="Rol">Nombre del rol, solo para mostrarlo; las decisiones se toman con los permisos.</param>
/// <param name="Permisos">Códigos de los permisos del rol.</param>
/// <param name="DebeCambiarContrasena">Indica que la contraseña es temporal y el frontend debe llevar al usuario a cambiarla.</param>
public sealed record UsuarioSesionDto(
    int Id,
    string NombreCompleto,
    string Email,
    string Rol,
    IReadOnlyList<string> Permisos,
    bool DebeCambiarContrasena);
