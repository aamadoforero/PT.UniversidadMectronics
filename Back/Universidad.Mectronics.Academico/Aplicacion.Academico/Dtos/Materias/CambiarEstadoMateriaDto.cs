namespace Aplicacion.Academico.Dtos.Materias;

/// <summary>
/// Datos para activar o desactivar una materia.
/// </summary>
/// <param name="Activo">Estado nuevo.</param>
public sealed record CambiarEstadoMateriaDto(bool? Activo);
