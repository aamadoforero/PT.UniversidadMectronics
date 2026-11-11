namespace Dominio.Seguridad.Autorizacion;

/// <summary>
/// Catálogo con todos los códigos de permiso del servicio de Seguridad, definidos en <see cref="CodigosPermisoUsuarios"/>
/// y <see cref="CodigosPermisoSuperAdministradores"/>. Los códigos son fijos porque cada uno corresponde a una funcionalidad
/// programada; qué rol recibe cada permiso se configura en <c>Seguridad.RolPermiso</c>.
/// </summary>
public static class CodigosPermiso
{
    /// <summary>
    /// Todos los códigos de este servicio. Sirve para registrar una política de autorización por permiso
    /// y para verificar al arrancar que todos existan en la base de datos.
    /// </summary>
    public static IReadOnlyList<string> Todos { get; } =
    [
        CodigosPermisoUsuarios.Consultar,
        CodigosPermisoUsuarios.Crear,
        CodigosPermisoUsuarios.Editar,
        CodigosPermisoUsuarios.CambiarEstado,
        CodigosPermisoUsuarios.RestablecerContrasena,
        CodigosPermisoUsuarios.CambiarRol,
        CodigosPermisoSuperAdministradores.Gestionar
    ];
}
