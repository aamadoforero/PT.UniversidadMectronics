namespace Aplicacion.Seguridad.Dtos.CuentaPropia;

/// <summary>
/// Datos académicos de una cuenta cuyo rol tiene perfil de estudiante.
/// </summary>
/// <param name="ProgramaId">Programa académico en el que está matriculado.</param>
/// <param name="NombrePrograma">Nombre del programa, para mostrarlo sin consultar el catálogo.</param>
/// <param name="UbicacionSemestral">Semestre que cursa, entre 1 y 10.</param>
public sealed record PerfilEstudianteDto(int ProgramaId, string? NombrePrograma, int UbicacionSemestral);
