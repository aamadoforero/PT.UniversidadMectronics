namespace Universidad.Mectronics.Transversal.Autenticacion;

/// <summary>
/// Nombres de los claims del token de acceso. Seguridad los escribe al emitir el token y todos los microservicios los leen
/// al validarlo, así que deben coincidir exactamente en ambos lados.
/// </summary>
public static class NombresClaim
{
    /// <summary>Identificador de la cuenta (claim estándar <c>sub</c>).</summary>
    public const string UsuarioId = "sub";

    /// <summary>Nombre completo de la cuenta (claim estándar <c>name</c>).</summary>
    public const string Nombre = "name";

    /// <summary>Correo de inicio de sesión (claim estándar <c>email</c>).</summary>
    public const string Email = "email";

    /// <summary>Nombre del rol; es informativo, la autorización se decide con <see cref="Permiso"/>.</summary>
    public const string Rol = "rol";

    /// <summary>Código de un permiso del rol; el token lleva un claim de este tipo por cada permiso.</summary>
    public const string Permiso = "permiso";

    /// <summary>Indica (<c>true</c> o <c>false</c>) que la contraseña es temporal y debe cambiarse antes de usar el sistema.</summary>
    public const string DebeCambiarContrasena = "debe_cambiar_contrasena";

    /// <summary>Huella de la contraseña vigente al emitir el token; deja de coincidir cuando la contraseña cambia.</summary>
    public const string HuellaContrasena = "huella";
}
