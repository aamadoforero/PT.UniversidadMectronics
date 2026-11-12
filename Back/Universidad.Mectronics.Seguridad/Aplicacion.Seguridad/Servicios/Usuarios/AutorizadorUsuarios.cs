using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>
/// Reglas de alcance de la administración de cuentas: nadie puede desactivar ni restablecer la contraseña de su propia
/// cuenta.
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
    /// Indica si la cuenta indicada es la de quien hace la petición.
    /// </summary>
    /// <param name="objetivo">Cuenta sobre la que se quiere operar.</param>
    /// <returns><see langword="true"/> si es su propia cuenta.</returns>
    public bool EsCuentaPropia(Usuario objetivo) => usuarioActual.EstaAutenticado && usuarioActual.Id == objetivo.Id;
}
