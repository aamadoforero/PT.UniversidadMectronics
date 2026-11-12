using Aplicacion.Seguridad.Dtos.Usuarios;
using Universidad.Mectronics.Transversal.Paginacion;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>
/// Casos de uso de lectura de la administración de cuentas. Exigen el permiso <c>usuarios.consultar</c>.
/// </summary>
public interface IServicioConsultaUsuarios
{
    /// <summary>
    /// Busca cuentas con filtros opcionales y devuelve una página de resultados, ordenada por nombre.
    /// </summary>
    /// <param name="criterios">Filtros y paginación.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La página de cuentas, o un error de validación o de falta de permiso.</returns>
    Task<Resultado<Pagina<UsuarioResumenDto>>> BuscarAsync(BuscarUsuariosDto criterios, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene el detalle de una cuenta.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El detalle, o un error de cuenta inexistente o de falta de permiso.</returns>
    Task<Resultado<UsuarioDetalleDto>> ObtenerAsync(int id, CancellationToken cancelacion);
}
