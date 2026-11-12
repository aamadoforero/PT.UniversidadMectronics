using Aplicacion.Academico.Dtos.Estudiantes;
using Aplicacion.Academico.Dtos.Inscripciones;
using Dominio.Academico.Entidades;
using Dominio.Academico.Politicas;

namespace Aplicacion.Academico.Mapeos;

/// <summary>
/// Conversión de estudiantes e inscripciones a sus DTO.
/// </summary>
public static class ExtensionesMapeoInscripcion
{
    /// <summary>
    /// Convierte una inscripción en su DTO.
    /// </summary>
    /// <param name="inscripcion">Inscripción con su materia y el profesor de la materia.</param>
    /// <returns>El DTO de la inscripción.</returns>
    public static InscripcionDto ConvertirAInscripcionDto(this Inscripcion inscripcion) => new(
        inscripcion.MateriaId,
        inscripcion.Materia.Nombre,
        inscripcion.Materia.Creditos,
        inscripcion.Materia.ProfesorId,
        inscripcion.Materia.Profesor.Usuario.NombreCompleto,
        inscripcion.FechaInscripcion);

    /// <summary>
    /// Convierte las inscripciones vigentes del estudiante en su DTO, con los límites del programa.
    /// </summary>
    /// <param name="estudiante">Estudiante con sus inscripciones vigentes.</param>
    /// <returns>El DTO de las inscripciones propias.</returns>
    public static InscripcionesPropiasDto ConvertirAInscripcionesPropiasDto(this Estudiante estudiante) => new(
        estudiante.InscripcionesActivas
            .OrderBy(inscripcion => inscripcion.Materia.Nombre, StringComparer.CurrentCulture)
            .Select(inscripcion => inscripcion.ConvertirAInscripcionDto())
            .ToList(),
        estudiante.CreditosInscritos,
        PoliticaInscripcion.MaximoCreditos,
        PoliticaInscripcion.MaximoMaterias);

    /// <summary>
    /// Convierte un estudiante en el resumen visible para los demás (R8).
    /// </summary>
    /// <param name="estudiante">Estudiante con su cuenta, su programa y sus inscripciones vigentes.</param>
    /// <returns>El resumen sin datos personales.</returns>
    public static EstudianteResumenDto ConvertirAEstudianteResumenDto(this Estudiante estudiante) => new(
        estudiante.Usuario.NombreCompleto,
        estudiante.Programa.Nombre,
        estudiante.InscripcionesActivas
            .Select(inscripcion => inscripcion.Materia.Nombre)
            .Order(StringComparer.CurrentCulture)
            .ToList(),
        estudiante.CreditosInscritos);
}
