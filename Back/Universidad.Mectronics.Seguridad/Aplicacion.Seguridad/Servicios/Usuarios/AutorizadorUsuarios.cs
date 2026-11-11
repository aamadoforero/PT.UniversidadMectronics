using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>
/// Reglas de alcance de la administración de cuentas: qué puede hacer quien hace la petición sobre cada cuenta y cada rol.
/// El sistema tiene un único superadministrador, creado por el script de la base de datos: su cuenta está protegida
/// (nadie la gestiona desde aquí; ella misma usa su perfil) y su rol no se asigna desde la aplicación.
/// </summary>
/// <param name="usuarioActual">Identidad y permisos de quien hace la petición.</param>
public sealed class AutorizadorUsuarios(IUsuarioActual usuarioActual)
{
    /// <summary>Identificador de quien hace la petición, para registrarlo en la auditoría.</summary>
    public int IdActor => usuarioActual.Id;

    /// <summary>
    /// Indica si quien hace la petición está autenticado y tiene el permiso.
    /// </summary>
    /// <param name="codigo">Código del permiso que exige la operación.</param>
    /// <returns><see langword="true"/> si tiene el permiso.</returns>
    public bool TienePermiso(string codigo) => usuarioActual.EstaAutenticado && usuarioActual.TienePermiso(codigo);

    /// <summary>
    /// Indica si la cuenta se puede gestionar desde la administración de cuentas. La del superadministrador está protegida.
    /// </summary>
    /// <param name="objetivo">Cuenta sobre la que se quiere operar.</param>
    /// <returns><see langword="true"/> si la cuenta no es de superadministración.</returns>
    public bool PuedeGestionar(Usuario objetivo) => !objetivo.Rol.EsSuperAdministrador;

    /// <summary>
    /// Indica si el rol se puede asignar desde la aplicación. El de superadministración solo lo asigna el script.
    /// </summary>
    /// <param name="rol">Rol que se quiere asignar.</param>
    /// <returns><see langword="true"/> si el rol no es de superadministración.</returns>
    public bool PuedeAsignar(Rol rol) => !rol.EsSuperAdministrador;

    /// <summary>
    /// Indica si la cuenta indicada es la de quien hace la petición.
    /// </summary>
    /// <param name="objetivo">Cuenta sobre la que se quiere operar.</param>
    /// <returns><see langword="true"/> si es su propia cuenta.</returns>
    public bool EsCuentaPropia(Usuario objetivo) => usuarioActual.EstaAutenticado && usuarioActual.Id == objetivo.Id;
}
