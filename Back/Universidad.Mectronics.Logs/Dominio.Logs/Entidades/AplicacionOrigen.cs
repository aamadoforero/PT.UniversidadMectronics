namespace Dominio.Logs.Entidades;

/// <summary>Aplicación que origina eventos de auditoría. Es de solo lectura.</summary>
public sealed class AplicacionOrigen
{
    /// <summary>Longitud máxima del nombre (columna <c>NVARCHAR(50)</c>).</summary>
    public const int LongitudMaximaNombre = 50;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="AplicacionOrigen"/>.</summary>
    private AplicacionOrigen()
    {
        Nombre = string.Empty;
    }

    /// <summary>Identificador.</summary>
    public byte Id { get; private set; }

    /// <summary>Nombre único de la aplicación.</summary>
    public string Nombre { get; private set; }
}
