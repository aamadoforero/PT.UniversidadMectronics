using Dominio.Academico.Excepciones;
using Dominio.Academico.Politicas;

namespace Dominio.Academico.Entidades;

/// <summary>
/// Perfil de estudiante y raíz del agregado de sus inscripciones. Las inscripciones solo cambian con
/// <see cref="Inscribir"/> y <see cref="CancelarInscripcion"/>.
/// </summary>
public sealed class Estudiante
{
    /// <summary>Inscripciones del estudiante.</summary>
    private readonly List<Inscripcion> _inscripciones = [];

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Estudiante"/>.</summary>
    private Estudiante()
    {
        Usuario = null!;
        Programa = null!;
    }

    /// <summary>Identificador del perfil.</summary>
    public int Id { get; private set; }

    /// <summary>Identificador de la cuenta del estudiante; es el claim <c>sub</c> del token de acceso.</summary>
    public int UsuarioId { get; private set; }

    /// <summary>Cuenta del estudiante, con su nombre.</summary>
    public Usuario Usuario { get; private set; }

    /// <summary>Programa en el que está matriculado.</summary>
    public int ProgramaId { get; private set; }

    /// <summary>Programa en el que está matriculado, con su nombre.</summary>
    public Programa Programa { get; private set; }

    /// <summary>Semestre que cursa.</summary>
    public int UbicacionSemestral { get; private set; }

    /// <summary>Inscripciones cargadas (las activas, salvo que el repositorio indique lo contrario).</summary>
    public IReadOnlyCollection<Inscripcion> Inscripciones => _inscripciones;

    /// <summary>Inscripciones vigentes.</summary>
    public IEnumerable<Inscripcion> InscripcionesActivas => _inscripciones.Where(inscripcion => inscripcion.Activo);

    /// <summary>Suma de los créditos de las materias inscritas.</summary>
    public int CreditosInscritos => InscripcionesActivas.Sum(inscripcion => inscripcion.Materia.Creditos);

    /// <summary>
    /// Inscribe una materia.
    /// </summary>
    /// <param name="materia">Materia a inscribir, con su profesor.</param>
    /// <param name="fecha">Momento de la inscripción.</param>
    /// <returns>La inscripción nueva.</returns>
    public Inscripcion Inscribir(Materia materia, DateTimeOffset fecha)
    {
        ArgumentNullException.ThrowIfNull(materia);
        MotivoRechazoInscripcion? motivo = PoliticaInscripcion.ObtenerMotivoRechazo(this, materia);
        if (motivo is not null)
        {
            throw new ExcepcionDominio($"La inscripción no cumple las reglas: {motivo}.");
        }

        Inscripcion inscripcion = new(materia, fecha);
        _inscripciones.Add(inscripcion);
        return inscripcion;
    }

    /// <summary>
    /// Cancela la inscripción vigente de una materia.
    /// </summary>
    /// <param name="materiaId">Materia a cancelar.</param>
    /// <param name="fecha">Momento de la cancelación.</param>
    /// <returns><see langword="false"/> si el estudiante no tiene esa materia inscrita.</returns>
    public bool CancelarInscripcion(int materiaId, DateTimeOffset fecha)
    {
        Inscripcion? inscripcion = InscripcionesActivas.FirstOrDefault(actual => actual.MateriaId == materiaId);
        if (inscripcion is null)
        {
            return false;
        }

        inscripcion.Cancelar(fecha);
        return true;
    }

    /// <summary>
    /// Indica si el estudiante tiene inscrita la materia.
    /// </summary>
    /// <param name="materiaId">Materia a buscar.</param>
    /// <returns><see langword="true"/> si tiene una inscripción vigente en ella.</returns>
    public bool TieneInscrita(int materiaId) => InscripcionesActivas.Any(inscripcion => inscripcion.MateriaId == materiaId);
}
