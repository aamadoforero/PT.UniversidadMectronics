namespace Dominio.Academico.Entidades;

/// <summary>
/// Programa académico. Agrupa las materias y los estudiantes; un estudiante solo inscribe materias de su programa.
/// </summary>
public sealed class Programa
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="Programa"/>.</summary>
    private Programa()
    {
        Nombre = string.Empty;
    }

    /// <summary>Identificador.</summary>
    public int Id { get; private set; }

    /// <summary>Nombre del programa.</summary>
    public string Nombre { get; private set; }

    /// <summary>Indica si el programa admite estudiantes y materias nuevas.</summary>
    public bool Activo { get; private set; }
}
