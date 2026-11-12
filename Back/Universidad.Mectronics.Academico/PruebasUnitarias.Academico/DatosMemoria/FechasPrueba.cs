namespace PruebasUnitarias.Academico.DatosMemoria;

/// <summary>
/// Fechas fijas de las pruebas, para que los resultados no dependan del reloj real.
/// </summary>
public static class FechasPrueba
{
    /// <summary>Fecha base de todas las pruebas.</summary>
    public static DateTimeOffset Inicio { get; } = new(2026, 11, 11, 12, 0, 0, TimeSpan.Zero);
}
