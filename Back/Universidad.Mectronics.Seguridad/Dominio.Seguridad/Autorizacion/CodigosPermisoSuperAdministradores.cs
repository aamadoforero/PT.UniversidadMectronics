using Dominio.Seguridad.Entidades;

namespace Dominio.Seguridad.Autorizacion;

/// <summary>
/// Código del permiso de acceso total. El código debe existir en <c>Seguridad.Permiso</c>
/// y forma parte del catálogo <see cref="CodigosPermiso.Todos"/>.
/// </summary>
public static class CodigosPermisoSuperAdministradores
{
    /// <summary>
    /// Acceso total. Identifica el rol de superadministración (<see cref="Rol.EsSuperAdministrador"/>): su única cuenta la crea
    /// el script, está protegida de la administración de cuentas y el rol no se asigna desde la aplicación.
    /// </summary>
    public const string Gestionar = "superadministradores.gestionar";
}
