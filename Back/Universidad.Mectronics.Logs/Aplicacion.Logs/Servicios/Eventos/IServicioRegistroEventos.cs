using Universidad.Mectronics.Transversal.Auditoria;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Logs.Servicios.Eventos;

/// <summary>
/// Caso de uso de registro de eventos enviados por los demás microservicios.
/// </summary>
public interface IServicioRegistroEventos
{
    /// <summary>
    /// Valida un evento y lo deja en la cola para guardarlo, a nombre de la aplicación autenticada.
    /// </summary>
    /// <param name="datos">Evento recibido.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Éxito si el evento quedó en la cola, o el error que impidió aceptarlo.</returns>
    Task<Resultado> RegistrarAsync(RegistrarEventoDto datos, CancellationToken cancelacion);
}
