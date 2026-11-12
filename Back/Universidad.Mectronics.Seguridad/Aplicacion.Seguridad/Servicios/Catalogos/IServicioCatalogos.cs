using Aplicacion.Seguridad.Dtos.Catalogos;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Catalogos;

/// <summary>
/// Catálogos de solo lectura que necesitan los formularios de administración de cuentas para llenar sus listas.
/// </summary>
public interface IServicioCatalogos
{
    /// <summary>
    /// Obtiene los roles que se pueden asignar desde la aplicación, ordenados por nombre.
    /// Exige <c>usuarios.crear</c>.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Los roles asignables, o un error de falta de permiso.</returns>
    Task<Resultado<IReadOnlyList<RolDto>>> ObtenerRolesAsignablesAsync(CancellationToken cancelacion);

    /// <summary>
    /// Obtiene los programas académicos activos. Exige <c>usuarios.crear</c> o <c>usuarios.editar</c>.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Los programas activos, o un error de falta de permiso.</returns>
    Task<Resultado<IReadOnlyList<ElementoCatalogoDto>>> ObtenerProgramasActivosAsync(CancellationToken cancelacion);

    /// <summary>
    /// Obtiene las categorías de profesor. Exige <c>usuarios.crear</c> o <c>usuarios.editar</c>.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Las categorías, o un error de falta de permiso.</returns>
    Task<Resultado<IReadOnlyList<ElementoCatalogoDto>>> ObtenerCategoriasProfesorAsync(CancellationToken cancelacion);
}
