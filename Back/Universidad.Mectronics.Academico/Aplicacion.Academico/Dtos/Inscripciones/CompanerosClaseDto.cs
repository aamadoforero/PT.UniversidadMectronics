namespace Aplicacion.Academico.Dtos.Inscripciones;

/// <summary>
/// Compañeros de una clase del estudiante. Solo lleva los nombres (R9): ni identificación, ni correo, ni identificadores.
/// </summary>
/// <param name="MateriaId">Identificador de la materia.</param>
/// <param name="Materia">Nombre de la materia.</param>
/// <param name="Companeros">Nombres de los demás estudiantes inscritos.</param>
public sealed record CompanerosClaseDto(int MateriaId, string Materia, IReadOnlyList<string> Companeros);
