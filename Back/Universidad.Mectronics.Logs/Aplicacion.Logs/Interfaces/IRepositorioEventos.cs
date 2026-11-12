using Dominio.Logs.Entidades;

namespace Aplicacion.Logs.Interfaces;

/// <summary>
/// Persistencia de los eventos. Solo agrega: un registro de auditoría no se modifica ni se elimina. La usa el proceso que
/// vacía la cola de eventos.
/// </summary>
public interface IRepositorioEventos
{
    /// <summary>
    /// Agrega un evento nuevo. Se guarda al llamar a <see cref="IUnidadTrabajo.GuardarCambiosAsync"/>.
    /// </summary>
    /// <param name="evento">Evento a registrar.</param>
    void Agregar(RegistroEvento evento);
}
