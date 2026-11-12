using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.Politicas;

/// <summary>
/// Número de inicios de sesión fallidos permitidos y duración del bloqueo.
/// </summary>
public sealed record PoliticaBloqueo
{
    /// <summary>
    /// Política estándar del sistema: 5 intentos fallidos bloquean la cuenta durante 15 minutos.
    /// </summary>
    public static PoliticaBloqueo Predeterminada { get; } = new(5, TimeSpan.FromMinutes(15));

    /// <summary>Inicializa una nueva instancia de la clase <see cref="PoliticaBloqueo"/>.</summary>
    /// <param name="intentosMaximos">Intentos fallidos consecutivos que provocan el bloqueo. Mínimo 1.</param>
    /// <param name="duracionBloqueo">Tiempo que dura el bloqueo. Debe ser mayor que cero.</param>
    public PoliticaBloqueo(int intentosMaximos, TimeSpan duracionBloqueo)
    {
        if (intentosMaximos < 1)
        {
            throw new ExcepcionDominio(
                "La política de bloqueo debe permitir al menos un intento.");
        }

        if (duracionBloqueo <= TimeSpan.Zero)
        {
            throw new ExcepcionDominio(
                "La duración del bloqueo debe ser mayor que cero.");
        }

        IntentosMaximos = intentosMaximos;
        DuracionBloqueo = duracionBloqueo;
    }

    /// <summary>Intentos fallidos consecutivos que provocan el bloqueo de la cuenta.</summary>
    public int IntentosMaximos { get; }

    /// <summary>Tiempo durante el cual la cuenta rechaza nuevos inicios de sesión.</summary>
    public TimeSpan DuracionBloqueo { get; }
}
