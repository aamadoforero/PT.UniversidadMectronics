namespace Aplicacion.Academico.Dtos.Inscripciones;

/// <summary>
/// Materia inscrita por el estudiante.
/// </summary>
/// <param name="MateriaId">Identificador de la materia.</param>
/// <param name="Materia">Nombre de la materia.</param>
/// <param name="Creditos">Créditos de la materia.</param>
/// <param name="ProfesorId">Profesor que la dicta; permite al cliente aplicar la regla de no repetir profesor (R7) por identificador.</param>
/// <param name="Profesor">Nombre del profesor que la dicta.</param>
/// <param name="FechaInscripcion">Momento de la inscripción, en UTC.</param>
public sealed record InscripcionDto(int MateriaId, string Materia, int Creditos, int ProfesorId, string Profesor, DateTimeOffset FechaInscripcion);
