namespace Dominio.Seguridad.Entidades;

/// <summary>
/// Inscripción de un estudiante a una materia. Seguridad solo la usa para cancelar las inscripciones vigentes al
/// desactivar la cuenta del estudiante.
/// </summary>
public sealed class Inscripcion
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="Inscripcion"/>.</summary>
    private Inscripcion()
    {
    }

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Inscripcion"/>.</summary>
    /// <param name="estudianteId">Perfil de estudiante dueño de la inscripción.</param>
    internal Inscripcion(int estudianteId)
    {
        EstudianteId = estudianteId;
        Activo = true;
    }

    /// <summary>Identificador.</summary>
    public int Id { get; private set; }

    /// <summary>Perfil de estudiante dueño de la inscripción.</summary>
    public int EstudianteId { get; private set; }

    /// <summary>Indica si la inscripción está vigente; una cancelada queda como historial.</summary>
    public bool Activo { get; private set; }

    /// <summary>Momento de la cancelación, en UTC.</summary>
    public DateTimeOffset? FechaModificacion { get; private set; }

    /// <summary>
    /// Cancela la inscripción: la deja inactiva con la fecha de cancelación.
    /// </summary>
    /// <param name="fechaUtc">Momento de la cancelación.</param>
    public void Cancelar(DateTimeOffset fechaUtc)
    {
        Activo = false;
        FechaModificacion = fechaUtc.ToUniversalTime();
    }
}
