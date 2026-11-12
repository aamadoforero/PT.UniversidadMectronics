using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Seguridad.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IRepositorioRoles"/> con Entity Framework.
/// </summary>
/// <param name="contexto">Contexto de la petición en curso.</param>
internal sealed class RepositorioRoles(ContextoSeguridad contexto) : IRepositorioRoles
{
    /// <summary>Obtiene un rol con sus permisos, con seguimiento de cambios.</summary>
    /// <param name="id">Identificador del rol.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El rol, o <see langword="null"/> si no existe.</returns>
    public Task<Rol?> ObtenerPorIdAsync(byte id, CancellationToken cancelacion) =>
        contexto.Roles.SingleOrDefaultAsync(rol => rol.Id == id, cancelacion);

    /// <summary>
    /// Obtiene todos los roles con sus permisos, ordenados por nombre, sin seguimiento porque solo se leen.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Los roles del sistema.</returns>
    public async Task<IReadOnlyList<Rol>> ObtenerTodosAsync(CancellationToken cancelacion) =>
        await contexto.Roles.AsNoTracking().OrderBy(rol => rol.Nombre).ToListAsync(cancelacion);
}
