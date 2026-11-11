using Aplicacion.Seguridad.Interfaces;
using PruebasUnitarias.Seguridad.DatosMemoria;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>
/// Reloj detenido en <see cref="FechasPrueba.Inicio"/>, para que las fechas de las pruebas sean predecibles.
/// </summary>
public sealed class RelojFijo : IReloj
{
    /// <summary>Hora actual simulada.</summary>
    public DateTimeOffset AhoraUtc { get; } = FechasPrueba.Inicio;
}
