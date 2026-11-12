using Aplicacion.Academico.Dtos.Materias;
using Aplicacion.Academico.Dtos.Profesores;
using Dominio.Academico.Entidades;
using Dominio.Academico.Politicas;

namespace Aplicacion.Academico.Mapeos;

/// <summary>
/// Conversión de materias y profesores a sus DTO.
/// </summary>
public static class ExtensionesMapeoMateria
{
    /// <summary>
    /// Convierte una materia en su DTO.
    /// </summary>
    /// <param name="materia">Materia con su profesor.</param>
    /// <returns>El DTO de la materia.</returns>
    public static MateriaDto ConvertirAMateriaDto(this Materia materia) => new(
        materia.Id,
        materia.Nombre,
        materia.Creditos,
        materia.ProgramaId,
        materia.ProfesorId,
        materia.Profesor.Usuario.NombreCompleto,
        materia.Activo);

    /// <summary>
    /// Convierte una materia en su DTO con el profesor indicado, para cuando la materia recién se creó o cambió de profesor y
    /// su navegación aún no está cargada.
    /// </summary>
    /// <param name="materia">Materia.</param>
    /// <param name="profesor">Profesor que la dicta, con su cuenta.</param>
    /// <returns>El DTO de la materia.</returns>
    public static MateriaDto ConvertirAMateriaDto(this Materia materia, Profesor profesor) => new(
        materia.Id,
        materia.Nombre,
        materia.Creditos,
        materia.ProgramaId,
        materia.ProfesorId,
        profesor.Usuario.NombreCompleto,
        materia.Activo);

    /// <summary>
    /// Convierte un profesor en su DTO.
    /// </summary>
    /// <param name="profesor">Profesor con su cuenta.</param>
    /// <param name="materiasAsignadas">Materias activas que dicta.</param>
    /// <returns>El DTO del profesor.</returns>
    public static ProfesorDto ConvertirAProfesorDto(this Profesor profesor, int materiasAsignadas) => new(
        profesor.Id,
        profesor.Usuario.NombreCompleto,
        materiasAsignadas,
        PoliticaMateria.MaximoMateriasPorProfesor);
}
