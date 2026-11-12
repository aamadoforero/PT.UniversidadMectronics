namespace Aplicacion.Academico.Dtos.Inscripciones;

/// <summary>
/// Datos para que el estudiante inscriba una materia.
/// </summary>
/// <param name="MateriaId">Materia a inscribir.</param>
public sealed record InscribirMateriaDto(int? MateriaId);
