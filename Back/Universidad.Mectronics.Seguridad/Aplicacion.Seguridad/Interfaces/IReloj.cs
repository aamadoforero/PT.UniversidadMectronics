namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Fuente de la hora actual en UTC. Permite fijar la hora en las pruebas.
/// </summary>
public interface IReloj
{
    /// <summary>Fecha y hora actual en UTC (desfase +00:00).</summary>
    DateTimeOffset AhoraUtc { get; }
}
