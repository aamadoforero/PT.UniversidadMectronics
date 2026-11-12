namespace Aplicacion.Academico.Dtos.Estudiantes;

/// <summary>
/// Criterios de búsqueda de estudiantes.
/// </summary>
/// <param name="Texto">Texto a buscar en el nombre, o <see langword="null"/> para todos.</param>
/// <param name="NumeroPagina">Página solicitada, desde 1.</param>
/// <param name="TamanoPagina">Estudiantes por página.</param>
public sealed record BuscarEstudiantesDto(string? Texto, int NumeroPagina = 1, int TamanoPagina = 20);
