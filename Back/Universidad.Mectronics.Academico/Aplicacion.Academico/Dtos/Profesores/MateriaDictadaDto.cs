namespace Aplicacion.Academico.Dtos.Profesores;

/// <summary>
/// Materia que dicta el profesor autenticado, con sus estudiantes inscritos. Solo lleva los nombres de los estudiantes.
/// </summary>
/// <param name="MateriaId">Identificador de la materia.</param>
/// <param name="Materia">Nombre de la materia.</param>
/// <param name="Creditos">Créditos de la materia.</param>
/// <param name="TotalInscritos">Número de estudiantes inscritos.</param>
/// <param name="Estudiantes">Nombres de los estudiantes inscritos, en orden alfabético.</param>
public sealed record MateriaDictadaDto(int MateriaId, string Materia, int Creditos, int TotalInscritos, IReadOnlyList<string> Estudiantes);
