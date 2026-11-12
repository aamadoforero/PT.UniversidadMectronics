using Aplicacion.Academico.Interfaces;
using PruebasUnitarias.Academico.DatosMemoria;

namespace PruebasUnitarias.Academico.Simuladores;

/// <summary>
/// Reloj detenido en <see cref="FechasPrueba.Inicio"/>, para que las fechas de las pruebas sean predecibles.
/// </summary>
public sealed class RelojFijo : IReloj
{
    /// <summary>Hora actual simulada.</summary>
    public DateTimeOffset AhoraUtc { get; } = FechasPrueba.Inicio;
}
