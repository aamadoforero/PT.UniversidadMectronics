namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Fuente de la hora actual. Existe para que ningún caso de uso lea el reloj del sistema directamente: así todas las
/// fechas se generan en UTC y las pruebas pueden fijar la hora.
/// </summary>
public interface IReloj
{
    /// <summary>Fecha y hora actual en UTC (desfase +00:00).</summary>
    DateTimeOffset AhoraUtc { get; }
}
