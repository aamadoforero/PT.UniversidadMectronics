namespace Dominio.Academico.Entidades;

/// <summary>
/// Inscripción de un estudiante en una materia. Al cancelarse queda inactiva como historial.
/// </summary>
public sealed class Inscripcion
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="Inscripcion"/>.</summary>
    private Inscripcion()
    {
        Materia = null!;
    }

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Inscripcion"/>.</summary>
    /// <param name="materia">Materia inscrita.</param>
    /// <param name="fecha">Momento de la inscripción.</param>
    internal Inscripcion(Materia materia, DateTimeOffset fecha)
    {
        Materia = materia;
        MateriaId = materia.Id;
        Activo = true;
        FechaInscripcion = fecha.ToUniversalTime();
    }

    /// <summary>Identificador.</summary>
    public int Id { get; private set; }

    /// <summary>Estudiante inscrito.</summary>
    public int EstudianteId { get; private set; }

    /// <summary>Identificador de la materia inscrita.</summary>
    public int MateriaId { get; private set; }

    /// <summary>Materia inscrita, con su profesor y sus créditos.</summary>
    public Materia Materia { get; private set; }

    /// <summary>Indica si la inscripción está vigente.</summary>
    public bool Activo { get; private set; }

    /// <summary>Momento de la inscripción, en UTC.</summary>
    public DateTimeOffset FechaInscripcion { get; private set; }

    /// <summary>Momento de la cancelación, en UTC.</summary>
    public DateTimeOffset? FechaModificacion { get; private set; }

    /// <summary>
    /// Cancela la inscripción.
    /// </summary>
    /// <param name="fecha">Momento de la cancelación.</param>
    internal void Cancelar(DateTimeOffset fecha)
    {
        Activo = false;
        FechaModificacion = fecha.ToUniversalTime();
    }
}
