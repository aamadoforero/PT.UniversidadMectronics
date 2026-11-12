namespace Aplicacion.Academico.Dtos.Inscripciones;

/// <summary>
/// Inscripciones vigentes del estudiante y su avance frente a los límites del programa.
/// </summary>
/// <param name="Materias">Materias inscritas.</param>
/// <param name="CreditosInscritos">Suma de los créditos inscritos.</param>
/// <param name="CreditosMaximos">Créditos máximos del programa.</param>
/// <param name="MateriasMaximas">Máximo de materias que se pueden inscribir.</param>
public sealed record InscripcionesPropiasDto(IReadOnlyList<InscripcionDto> Materias, int CreditosInscritos, int CreditosMaximos, int MateriasMaximas);
