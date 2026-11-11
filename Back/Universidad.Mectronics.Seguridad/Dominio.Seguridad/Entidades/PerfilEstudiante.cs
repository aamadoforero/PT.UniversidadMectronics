using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.Entidades;

/// <summary>
/// Datos institucionales de una cuenta cuyo rol tiene <see cref="TipoPerfil.Estudiante"/>.
/// </summary>
public sealed class PerfilEstudiante
{
    /// <summary>Primer semestre permitido.</summary>
    public const int UbicacionSemestralMinima = 1;

    /// <summary>Último semestre permitido (coincide con el CHECK <c>CK_Estudiante_UbicacionSemestral</c>).</summary>
    public const int UbicacionSemestralMaxima = 10;

    /// <summary>Constructor requerido por Entity Framework para materializar el registro.</summary>
    private PerfilEstudiante()
    {
    }

    /// <summary>
    /// Crea el perfil. Solo lo usa <see cref="Usuario.CrearEstudiante"/>.
    /// </summary>
    /// <param name="programaId">Programa académico del estudiante.</param>
    /// <param name="ubicacionSemestral">Semestre que cursa, entre 1 y 10.</param>
    internal PerfilEstudiante(int programaId, int ubicacionSemestral)
    {
        Actualizar(programaId, ubicacionSemestral);
    }

    /// <summary>Identificador generado por la base de datos (PK).</summary>
    public int Id { get; private set; }

    /// <summary>Cuenta a la que pertenece el perfil (relación 1 a 1). Lo asigna Entity Framework al guardar.</summary>
    public int UsuarioId { get; private set; }

    /// <summary>Programa académico en el que está matriculado (FK a <c>Academico.Programa</c>).</summary>
    public int ProgramaId { get; private set; }

    /// <summary>Semestre que cursa actualmente, entre <see cref="UbicacionSemestralMinima"/> y <see cref="UbicacionSemestralMaxima"/>.</summary>
    public int UbicacionSemestral { get; private set; }

    /// <summary>
    /// Cambia el programa y el semestre después de validarlos.
    /// </summary>
    /// <param name="programaId">Programa académico; debe ser mayor que cero.</param>
    /// <param name="ubicacionSemestral">Semestre, entre 1 y 10.</param>
    internal void Actualizar(int programaId, int ubicacionSemestral)
    {
        if (programaId <= 0)
        {
            throw new ExcepcionDominio(
                "El estudiante debe pertenecer a un programa académico.");
        }

        if (ubicacionSemestral is < UbicacionSemestralMinima or > UbicacionSemestralMaxima)
        {
            throw new ExcepcionDominio(
                $"La ubicación semestral debe estar entre {UbicacionSemestralMinima} y {UbicacionSemestralMaxima}.");
        }

        ProgramaId = programaId;
        UbicacionSemestral = ubicacionSemestral;
    }
}
