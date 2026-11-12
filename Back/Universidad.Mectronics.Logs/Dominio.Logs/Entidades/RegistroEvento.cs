using Dominio.Logs.Excepciones;

namespace Dominio.Logs.Entidades;

/// <summary>
/// Evento de auditoría o error de un microservicio. Es inmutable: solo se crea con <see cref="Crear"/>.
/// </summary>
public sealed class RegistroEvento
{
    /// <summary>Longitud máxima del mensaje. La columna es <c>NVARCHAR(MAX)</c>; el límite evita registros desmedidos.</summary>
    public const int LongitudMaximaMensaje = 4_000;

    /// <summary>Longitud máxima del detalle de la excepción (stack trace incluido).</summary>
    public const int LongitudMaximaExcepcion = 64_000;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="RegistroEvento"/>.</summary>
    private RegistroEvento()
    {
        Mensaje = string.Empty;
    }

    /// <summary>Inicializa una nueva instancia de la clase <see cref="RegistroEvento"/>.</summary>
    /// <param name="tipoEventoId">Identificador del tipo de evento.</param>
    /// <param name="aplicacionId">Identificador de la aplicación de origen.</param>
    /// <param name="mensaje">Descripción del evento, sin espacios en los extremos.</param>
    /// <param name="excepcion">Detalle técnico, o <see langword="null"/>.</param>
    /// <param name="fechaEvento">Momento del evento, en UTC.</param>
    private RegistroEvento(byte tipoEventoId, byte aplicacionId, string mensaje, string? excepcion, DateTimeOffset fechaEvento)
    {
        TipoEventoId = tipoEventoId;
        AplicacionId = aplicacionId;
        Mensaje = mensaje;
        Excepcion = excepcion;
        FechaEvento = fechaEvento;
    }

    /// <summary>Identificador.</summary>
    public int Id { get; private set; }

    /// <summary>Momento en que ocurrió el evento en el microservicio de origen, en UTC.</summary>
    public DateTimeOffset FechaEvento { get; private set; }

    /// <summary>Identificador del tipo de evento.</summary>
    public byte TipoEventoId { get; private set; }

    /// <summary>Identificador de la aplicación que lo originó.</summary>
    public byte AplicacionId { get; private set; }

    /// <summary>Descripción del evento.</summary>
    public string Mensaje { get; private set; }

    /// <summary>Detalle técnico de la excepción, si lo hay.</summary>
    public string? Excepcion { get; private set; }

    /// <summary>
    /// Crea un evento. Quita los espacios de los extremos del mensaje, descarta un detalle de excepción vacío y lleva la fecha a UTC.
    /// </summary>
    /// <param name="tipoEventoId">Identificador del tipo de evento.</param>
    /// <param name="aplicacionId">Identificador de la aplicación de origen.</param>
    /// <param name="mensaje">Descripción del evento; obligatoria.</param>
    /// <param name="excepcion">Detalle técnico de la excepción, o <see langword="null"/>.</param>
    /// <param name="fechaEvento">Momento en que ocurrió.</param>
    /// <returns>El evento listo para guardar.</returns>
    public static RegistroEvento Crear(byte tipoEventoId, byte aplicacionId, string mensaje, string? excepcion, DateTimeOffset fechaEvento)
    {
        if (tipoEventoId == 0)
        {
            throw new ExcepcionDominio("El tipo de evento es obligatorio.");
        }

        if (aplicacionId == 0)
        {
            throw new ExcepcionDominio("La aplicación de origen es obligatoria.");
        }

        string normalizado = mensaje?.Trim() ?? string.Empty;
        if (normalizado.Length == 0 || normalizado.Length > LongitudMaximaMensaje)
        {
            throw new ExcepcionDominio($"El mensaje es obligatorio (máximo {LongitudMaximaMensaje} caracteres).");
        }

        if (excepcion is { Length: > LongitudMaximaExcepcion })
        {
            throw new ExcepcionDominio($"El detalle de la excepción admite máximo {LongitudMaximaExcepcion} caracteres.");
        }

        return new RegistroEvento(
            tipoEventoId,
            aplicacionId,
            normalizado,
            string.IsNullOrWhiteSpace(excepcion) ? null : excepcion,
            fechaEvento.ToUniversalTime());
    }
}
