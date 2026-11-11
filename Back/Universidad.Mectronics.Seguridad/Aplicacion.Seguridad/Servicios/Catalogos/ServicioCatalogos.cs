using Aplicacion.Seguridad.Dtos.Catalogos;
using Aplicacion.Seguridad.Errores;
using Aplicacion.Seguridad.Interfaces;
using Aplicacion.Seguridad.Mapeos;
using Dominio.Seguridad.Autorizacion;
using Dominio.Seguridad.Entidades;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Catalogos;

/// <summary>
/// Implementación de la consulta de catálogos. Cada catálogo exige alguno de los permisos de los formularios que lo usan.
/// </summary>
/// <param name="usuarioActual">Identidad y permisos de quien hace la petición.</param>
/// <param name="repositorioRoles">Acceso a los roles.</param>
/// <param name="consultaCatalogos">Consulta de programas y categorías.</param>
public sealed class ServicioCatalogos(
    IUsuarioActual usuarioActual,
    IRepositorioRoles repositorioRoles,
    IConsultaCatalogos consultaCatalogos) : IServicioCatalogos
{
    /// <summary>
    /// Obtiene los roles que se pueden asignar desde la aplicación.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Los roles asignables, o un error de falta de permiso.</returns>
    public async Task<Resultado<IReadOnlyList<RolDto>>> ObtenerRolesAsignablesAsync(CancellationToken cancelacion)
    {
        if (!TieneAlgunPermiso(CodigosPermisoUsuarios.Crear, CodigosPermisoUsuarios.CambiarRol))
        {
            return ErroresCatalogos.CrearSinPermiso();
        }

        IReadOnlyList<Rol> roles = await repositorioRoles.ObtenerTodosAsync(cancelacion);
        IReadOnlyList<RolDto> asignables = roles
            .Where(rol => !rol.EsSuperAdministrador)
            .Select(rol => rol.ConvertirARolDto())
            .ToList();

        return Resultado.CrearExito(asignables);
    }

    /// <summary>
    /// Obtiene los programas académicos activos.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Los programas activos, o un error de falta de permiso.</returns>
    public async Task<Resultado<IReadOnlyList<ElementoCatalogoDto>>> ObtenerProgramasActivosAsync(CancellationToken cancelacion)
    {
        if (!TieneAlgunPermiso(CodigosPermisoUsuarios.Crear, CodigosPermisoUsuarios.Editar))
        {
            return ErroresCatalogos.CrearSinPermiso();
        }

        IReadOnlyList<ElementoCatalogoDto> programas = await consultaCatalogos.ObtenerProgramasActivosAsync(cancelacion);
        return Resultado.CrearExito(programas);
    }

    /// <summary>
    /// Obtiene las categorías de profesor.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Las categorías, o un error de falta de permiso.</returns>
    public async Task<Resultado<IReadOnlyList<ElementoCatalogoDto>>> ObtenerCategoriasProfesorAsync(CancellationToken cancelacion)
    {
        if (!TieneAlgunPermiso(CodigosPermisoUsuarios.Crear, CodigosPermisoUsuarios.Editar))
        {
            return ErroresCatalogos.CrearSinPermiso();
        }

        IReadOnlyList<ElementoCatalogoDto> categorias = await consultaCatalogos.ObtenerCategoriasProfesorAsync(cancelacion);
        return Resultado.CrearExito(categorias);
    }

    /// <summary>
    /// Indica si quien hace la petición está autenticado y tiene al menos uno de los permisos.
    /// </summary>
    /// <param name="codigos">Permisos que habilitan el catálogo.</param>
    /// <returns><see langword="true"/> si tiene alguno de ellos.</returns>
    private bool TieneAlgunPermiso(params string[] codigos) =>
        usuarioActual.EstaAutenticado && codigos.Any(usuarioActual.TienePermiso);
}
