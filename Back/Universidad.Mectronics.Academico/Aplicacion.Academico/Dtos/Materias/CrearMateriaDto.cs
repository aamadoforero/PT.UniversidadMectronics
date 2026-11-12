namespace Aplicacion.Academico.Dtos.Materias;

/// <summary>
/// Datos para crear una materia. Los créditos no se reciben: toda materia vale 3 (R4).
/// </summary>
/// <param name="Nombre">Nombre, único dentro del programa.</param>
/// <param name="ProgramaId">Programa al que pertenece.</param>
/// <param name="ProfesorId">Profesor que la dicta.</param>
public sealed record CrearMateriaDto(string? Nombre, int? ProgramaId, int? ProfesorId);
