namespace Aplicacion.Academico.Dtos.Profesores;

/// <summary>
/// Profesor disponible para asignar a materias.
/// </summary>
/// <param name="Id">Identificador del perfil de profesor.</param>
/// <param name="Nombre">Nombre del profesor.</param>
/// <param name="MateriasAsignadas">Materias activas que dicta.</param>
/// <param name="MateriasMaximas">Máximo de materias que puede dictar.</param>
public sealed record ProfesorDto(int Id, string Nombre, int MateriasAsignadas, int MateriasMaximas);
