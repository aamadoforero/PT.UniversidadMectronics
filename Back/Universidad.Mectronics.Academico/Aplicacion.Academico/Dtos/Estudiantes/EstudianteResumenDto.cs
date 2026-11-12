namespace Aplicacion.Academico.Dtos.Estudiantes;

/// <summary>
/// Registro de inscripción de un estudiante, visible para los demás (R8). Solo lleva datos académicos: ni identificación,
/// ni correo, ni identificadores de cuenta.
/// </summary>
/// <param name="Nombre">Nombre del estudiante.</param>
/// <param name="Programa">Programa en el que está matriculado.</param>
/// <param name="Materias">Nombres de las materias inscritas.</param>
/// <param name="CreditosInscritos">Suma de los créditos inscritos.</param>
public sealed record EstudianteResumenDto(string Nombre, string Programa, IReadOnlyList<string> Materias, int CreditosInscritos);
