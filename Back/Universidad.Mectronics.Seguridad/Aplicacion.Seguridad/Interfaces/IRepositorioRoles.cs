using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Acceso a los roles con sus permisos.
/// </summary>
public interface IRepositorioRoles
{
    /// <summary>
    /// Obtiene un rol con sus permisos.
    /// </summary>
    /// <param name="id">Identificador del rol.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El rol, o <see langword="null"/> si no existe.</returns>
    Task<Rol?> ObtenerPorIdAsync(byte id, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene todos los roles con sus permisos, ordenados por nombre.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Los roles del sistema.</returns>
    Task<IReadOnlyList<Rol>> ObtenerTodosAsync(CancellationToken cancelacion);
}
