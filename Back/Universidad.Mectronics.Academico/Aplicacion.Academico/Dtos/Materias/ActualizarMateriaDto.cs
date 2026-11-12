namespace Aplicacion.Academico.Dtos.Materias;

/// <summary>
/// Datos para editar una materia. El programa no se cambia (las inscripciones dependen de él) y los créditos son fijos.
/// </summary>
/// <param name="Nombre">Nombre nuevo.</param>
/// <param name="ProfesorId">Profesor nuevo.</param>
public sealed record ActualizarMateriaDto(string? Nombre, int? ProfesorId);
