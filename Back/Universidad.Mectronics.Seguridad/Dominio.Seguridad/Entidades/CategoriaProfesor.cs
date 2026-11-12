namespace Dominio.Seguridad.Entidades;

/// <summary>Categoría de profesor. Es de solo lectura en Seguridad.</summary>
public sealed class CategoriaProfesor
{
    /// <summary>Longitud máxima del nombre (columna <c>NVARCHAR(150)</c>).</summary>
    public const int LongitudMaximaNombre = 150;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="CategoriaProfesor"/>.</summary>
    private CategoriaProfesor()
    {
        Nombre = string.Empty;
    }

    /// <summary>Identificador.</summary>
    public byte Id { get; private set; }

    /// <summary>Nombre de la categoría.</summary>
    public string Nombre { get; private set; }
}
