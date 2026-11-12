using Dominio.Logs.Entidades;

namespace Aplicacion.Logs.Interfaces;

/// <summary>Cola de eventos validados pendientes de guardar.</summary>
public interface IColaEventos
{
    /// <summary>
    /// Agrega un evento a la cola sin esperar.
    /// </summary>
    /// <param name="evento">Evento listo para guardar.</param>
    /// <returns><see langword="false"/> si la cola ya no admite eventos porque el servicio se está deteniendo.</returns>
    bool Encolar(RegistroEvento evento);
}
