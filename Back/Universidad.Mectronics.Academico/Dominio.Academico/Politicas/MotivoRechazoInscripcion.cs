namespace Dominio.Academico.Politicas;

/// <summary>
/// Razón por la que <see cref="PoliticaInscripcion"/> rechaza una inscripción. La capa de Aplicación la traduce a un error
/// con un mensaje para el usuario.
/// </summary>
public enum MotivoRechazoInscripcion
{
    /// <summary>La materia está desactivada.</summary>
    MateriaInactiva = 1,

    /// <summary>La materia pertenece a otro programa.</summary>
    MateriaDeOtroPrograma = 2,

    /// <summary>El estudiante ya tiene inscrita la materia.</summary>
    MateriaYaInscrita = 3,

    /// <summary>El estudiante ya tiene el máximo de materias (R5).</summary>
    MaximoMateriasAlcanzado = 4,

    /// <summary>La materia haría superar el máximo de créditos del programa (R2, R4).</summary>
    MaximoCreditosSuperado = 5,

    /// <summary>El estudiante ya tiene otra materia con el mismo profesor (R7).</summary>
    ProfesorRepetido = 6
}
