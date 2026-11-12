using Aplicacion.Seguridad.Dtos.Usuarios;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>
/// Casos de uso que controlan el acceso de una cuenta: su estado y su contraseña. No se permiten sobre la propia
/// cuenta.
/// </summary>
public interface IServicioAccesoUsuarios
{
    /// <summary>
    /// Activa o desactiva una cuenta. Una cuenta desactivada no puede iniciar sesión y sus sesiones dejan de renovarse; si es
    /// un estudiante, se cancelan sus inscripciones vigentes.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="datos">Estado nuevo.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La cuenta actualizada, o un error.</returns>
    Task<Resultado<UsuarioDetalleDto>> CambiarEstadoAsync(int id, CambiarEstadoDto datos, CancellationToken cancelacion);

    /// <summary>
    /// Genera una contraseña temporal para una cuenta, la desbloquea e invalida sus sesiones anteriores.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La contraseña temporal, o un error.</returns>
    Task<Resultado<ContrasenaRestablecidaDto>> RestablecerContrasenaAsync(int id, CancellationToken cancelacion);
}
