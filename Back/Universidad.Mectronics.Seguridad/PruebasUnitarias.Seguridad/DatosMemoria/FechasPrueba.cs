namespace PruebasUnitarias.Seguridad.DatosMemoria;

/// <summary>
/// Fechas fijas para las pruebas. Usar siempre la misma hora hace que las reglas dependientes del tiempo (bloqueo,
/// vencimientos) den el mismo resultado en cada ejecución.
/// </summary>
public static class FechasPrueba
{
    /// <summary>Hora de referencia de todas las pruebas: 10 de noviembre de 2026, 12:00 UTC.</summary>
    public static DateTimeOffset Inicio { get; } = new(2026, 11, 10, 12, 0, 0, TimeSpan.Zero);
}
