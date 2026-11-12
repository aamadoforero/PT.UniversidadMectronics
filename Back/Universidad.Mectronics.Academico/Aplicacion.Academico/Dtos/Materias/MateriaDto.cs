namespace Aplicacion.Academico.Dtos.Materias;

/// <summary>
/// Materia con su profesor.
/// </summary>
/// <param name="Id">Identificador.</param>
/// <param name="Nombre">Nombre.</param>
/// <param name="Creditos">Créditos.</param>
/// <param name="ProgramaId">Programa al que pertenece.</param>
/// <param name="ProfesorId">Profesor que la dicta.</param>
/// <param name="Profesor">Nombre del profesor.</param>
/// <param name="Activo">Indica si admite inscripciones.</param>
public sealed record MateriaDto(int Id, string Nombre, int Creditos, int ProgramaId, int ProfesorId, string Profesor, bool Activo);
