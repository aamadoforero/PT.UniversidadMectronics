namespace PruebasUnitarias.Seguridad.DatosMemoria;

/// <summary>Fechas fijas para las pruebas.</summary>
public static class FechasPrueba
{
    /// <summary>Hora de referencia de todas las pruebas: 10 de noviembre de 2026, 12:00 UTC.</summary>
    public static DateTimeOffset Inicio { get; } = new(2026, 11, 10, 12, 0, 0, TimeSpan.Zero);
}
