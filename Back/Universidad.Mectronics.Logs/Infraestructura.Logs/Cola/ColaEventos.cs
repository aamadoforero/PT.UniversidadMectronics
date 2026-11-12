using System.Threading.Channels;
using Aplicacion.Logs.Interfaces;
using Dominio.Logs.Entidades;

namespace Infraestructura.Logs.Cola;

/// <summary>
/// Implementación de <see cref="IColaEventos"/> con un canal en memoria de capacidad limitada; si se llena, descarta
/// los eventos más antiguos.
/// </summary>
internal sealed class ColaEventos : IColaEventos
{
    /// <summary>Máximo de eventos pendientes de guardar.</summary>
    public const int Capacidad = 10_000;

    /// <summary>Canal con varios escritores (las peticiones) y un solo lector (<see cref="ProcesadorEventos"/>).</summary>
    private readonly Channel<RegistroEvento> _canal = Channel.CreateBounded<RegistroEvento>(
        new BoundedChannelOptions(Capacidad) { FullMode = BoundedChannelFullMode.DropOldest, SingleReader = true });

    /// <summary>Lectura de los eventos pendientes, para el proceso que los guarda.</summary>
    public ChannelReader<RegistroEvento> Lector => _canal.Reader;

    /// <summary>
    /// Agrega un evento a la cola sin esperar.
    /// </summary>
    /// <param name="evento">Evento listo para guardar.</param>
    /// <returns><see langword="false"/> si la cola ya fue cerrada.</returns>
    public bool Encolar(RegistroEvento evento) => _canal.Writer.TryWrite(evento);

    /// <summary>
    /// Cierra la cola: no admite eventos nuevos, pero los pendientes se siguen leyendo.
    /// </summary>
    public void Cerrar() => _canal.Writer.TryComplete();
}
