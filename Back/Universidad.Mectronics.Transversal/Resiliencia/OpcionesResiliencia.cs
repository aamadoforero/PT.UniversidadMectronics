namespace Universidad.Mectronics.Transversal.Resiliencia;

/// <summary>
/// Parámetros de la política de resiliencia de un cliente HTTP hacia otro microservicio.
/// </summary>
public sealed class OpcionesResiliencia
{
    /// <summary>Tiempo máximo de una llamada, incluidos todos sus reintentos.</summary>
    public TimeSpan TiempoMaximoTotal { get; init; } = TimeSpan.FromSeconds(20);

    /// <summary>Tiempo máximo de cada intento.</summary>
    public TimeSpan TiempoMaximoIntento { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>Reintentos después del primer intento fallido.</summary>
    public int MaximoReintentos { get; init; } = 3;

    /// <summary>Espera base entre reintentos; crece de forma exponencial con variación aleatoria.</summary>
    public TimeSpan EsperaBase { get; init; } = TimeSpan.FromSeconds(1);

    /// <summary>Proporción de fallos que abre el circuito.</summary>
    public double ProporcionFallos { get; init; } = 0.5;

    /// <summary>Mínimo de llamadas en la ventana de muestreo para evaluar la proporción de fallos.</summary>
    public int MinimoLlamadas { get; init; } = 5;

    /// <summary>Ventana de tiempo en la que se mide la proporción de fallos.</summary>
    public TimeSpan VentanaMuestreo { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>Tiempo que el circuito permanece abierto antes de volver a probar.</summary>
    public TimeSpan DuracionCorte { get; init; } = TimeSpan.FromSeconds(30);
}
