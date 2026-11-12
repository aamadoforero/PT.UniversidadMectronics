namespace Dominio.Logs.Entidades;

/// <summary>
/// Tipo de evento (Informativo, Advertencia o Error). Es de solo lectura.
/// </summary>
public sealed class TipoEvento
{
    /// <summary>Longitud máxima del nombre (columna <c>NVARCHAR(20)</c>).</summary>
    public const int LongitudMaximaNombre = 20;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="TipoEvento"/>.</summary>
    private TipoEvento()
    {
        Nombre = string.Empty;
    }

    /// <summary>Identificador.</summary>
    public byte Id { get; private set; }

    /// <summary>Nombre único del tipo, por ejemplo <c>Advertencia</c>.</summary>
    public string Nombre { get; private set; }
}
