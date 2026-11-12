using Dominio.Academico.Excepciones;
using Dominio.Academico.Politicas;

namespace Dominio.Academico.Entidades;

/// <summary>
/// Materia de un programa, dictada por un profesor. Toda materia vale <see cref="PoliticaMateria.CreditosPorMateria"/>
/// créditos.
/// </summary>
public sealed class Materia
{
    /// <summary>Longitud máxima del nombre (columna <c>NVARCHAR(100)</c>).</summary>
    public const int LongitudMaximaNombre = 100;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Materia"/>.</summary>
    private Materia()
    {
        Nombre = string.Empty;
        Profesor = null!;
    }

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Materia"/>.</summary>
    /// <param name="nombre">Nombre normalizado.</param>
    /// <param name="programaId">Programa al que pertenece.</param>
    /// <param name="profesorId">Profesor que la dicta.</param>
    /// <param name="fecha">Momento de la creación.</param>
    private Materia(string nombre, int programaId, int profesorId, DateTimeOffset fecha)
    {
        Nombre = nombre;
        Creditos = PoliticaMateria.CreditosPorMateria;
        ProgramaId = programaId;
        ProfesorId = profesorId;
        Activo = true;
        FechaCreacion = fecha;
        Profesor = null!;
    }

    /// <summary>Identificador.</summary>
    public int Id { get; private set; }

    /// <summary>Nombre de la materia, único dentro del programa.</summary>
    public string Nombre { get; private set; }

    /// <summary>Créditos que suma al estudiante que la inscribe; siempre <see cref="PoliticaMateria.CreditosPorMateria"/>.</summary>
    public int Creditos { get; private set; }

    /// <summary>Programa al que pertenece.</summary>
    public int ProgramaId { get; private set; }

    /// <summary>Identificador del profesor que la dicta.</summary>
    public int ProfesorId { get; private set; }

    /// <summary>Profesor que la dicta, con su nombre.</summary>
    public Profesor Profesor { get; private set; }

    /// <summary>Indica si admite inscripciones.</summary>
    public bool Activo { get; private set; }

    /// <summary>Momento de la creación, en UTC.</summary>
    public DateTimeOffset FechaCreacion { get; private set; }

    /// <summary>Momento de la última modificación, en UTC.</summary>
    public DateTimeOffset? FechaModificacion { get; private set; }

    /// <summary>
    /// Crea una materia activa de <see cref="PoliticaMateria.CreditosPorMateria"/> créditos.
    /// </summary>
    /// <param name="nombre">Nombre; se quitan los espacios de los extremos.</param>
    /// <param name="programaId">Programa al que pertenece.</param>
    /// <param name="profesorId">Profesor que la dicta.</param>
    /// <param name="fecha">Momento de la creación.</param>
    /// <returns>La materia nueva.</returns>
    public static Materia Crear(string nombre, int programaId, int profesorId, DateTimeOffset fecha)
    {
        if (programaId <= 0)
        {
            throw new ExcepcionDominio("El programa de la materia es obligatorio.");
        }

        return new Materia(NormalizarNombre(nombre), programaId, ValidarProfesor(profesorId), fecha.ToUniversalTime());
    }

    /// <summary>
    /// Actualiza el nombre y el profesor.
    /// </summary>
    /// <param name="nombre">Nombre nuevo.</param>
    /// <param name="profesorId">Profesor nuevo.</param>
    /// <param name="fecha">Momento de la modificación.</param>
    public void Actualizar(string nombre, int profesorId, DateTimeOffset fecha)
    {
        Nombre = NormalizarNombre(nombre);
        ProfesorId = ValidarProfesor(profesorId);
        FechaModificacion = fecha.ToUniversalTime();
    }

    /// <summary>
    /// Activa o desactiva la materia. Una materia inactiva no admite inscripciones nuevas.
    /// </summary>
    /// <param name="activo">Estado nuevo.</param>
    /// <param name="fecha">Momento de la modificación.</param>
    public void CambiarEstado(bool activo, DateTimeOffset fecha)
    {
        Activo = activo;
        FechaModificacion = fecha.ToUniversalTime();
    }

    /// <summary>
    /// Quita los espacios de los extremos y valida la longitud.
    /// </summary>
    /// <param name="nombre">Nombre recibido.</param>
    /// <returns>El nombre normalizado.</returns>
    private static string NormalizarNombre(string nombre)
    {
        string normalizado = nombre?.Trim() ?? string.Empty;
        if (normalizado.Length == 0 || normalizado.Length > LongitudMaximaNombre)
        {
            throw new ExcepcionDominio($"El nombre de la materia es obligatorio (máximo {LongitudMaximaNombre} caracteres).");
        }

        return normalizado;
    }

    /// <summary>
    /// Valida que haya un profesor asignado.
    /// </summary>
    /// <param name="profesorId">Profesor recibido.</param>
    /// <returns>El mismo identificador.</returns>
    private static int ValidarProfesor(int profesorId) => profesorId > 0
        ? profesorId
        : throw new ExcepcionDominio("El profesor de la materia es obligatorio.");
}
