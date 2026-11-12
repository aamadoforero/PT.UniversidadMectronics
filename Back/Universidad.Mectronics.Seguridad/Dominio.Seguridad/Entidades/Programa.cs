namespace Dominio.Seguridad.Entidades;

/// <summary>
/// Programa académico al que se asocia un estudiante. Es de solo lectura en Seguridad.
/// </summary>
public sealed class Programa
{
    /// <summary>Longitud máxima del nombre (columna <c>NVARCHAR(150)</c>).</summary>
    public const int LongitudMaximaNombre = 150;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Programa"/>.</summary>
    private Programa()
    {
        Nombre = string.Empty;
    }

    /// <summary>Identificador.</summary>
    public int Id { get; private set; }

    /// <summary>Nombre del programa.</summary>
    public string Nombre { get; private set; }

    /// <summary>Indica si el programa admite estudiantes.</summary>
    public bool Activo { get; private set; }
}
