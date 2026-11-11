using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;
using PruebasUnitarias.Seguridad.DatosMemoria;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>
/// Repositorio de roles en memoria con los roles de <see cref="FabricaRoles"/>.
/// </summary>
public sealed class RepositorioRolesEnMemoria : IRepositorioRoles
{
    /// <summary>Roles disponibles.</summary>
    private readonly IReadOnlyList<Rol> _roles = FabricaRoles.CrearTodos();

    /// <summary>Busca un rol por identificador.</summary>
    /// <param name="id">Identificador.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>El rol o <see langword="null"/>.</returns>
    public Task<Rol?> ObtenerPorIdAsync(byte id, CancellationToken cancelacion) =>
        Task.FromResult(_roles.FirstOrDefault(rol => rol.Id == id));

    /// <summary>Devuelve todos los roles.</summary>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Los roles.</returns>
    public Task<IReadOnlyList<Rol>> ObtenerTodosAsync(CancellationToken cancelacion) => Task.FromResult(_roles);
}
