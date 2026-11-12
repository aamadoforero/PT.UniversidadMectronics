namespace Dominio.Seguridad.Autorizacion;

/// <summary>Códigos de permiso del servicio de Seguridad.</summary>
public static class CodigosPermiso
{
    /// <summary>Todos los códigos de permiso de este servicio.</summary>
    public static IReadOnlyList<string> Todos { get; } =
    [
        CodigosPermisoUsuarios.Consultar,
        CodigosPermisoUsuarios.Crear,
        CodigosPermisoUsuarios.Editar,
        CodigosPermisoUsuarios.CambiarEstado,
        CodigosPermisoUsuarios.RestablecerContrasena
    ];
}
