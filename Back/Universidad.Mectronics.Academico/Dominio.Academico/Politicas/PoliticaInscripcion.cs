using Dominio.Academico.Entidades;

namespace Dominio.Academico.Politicas;

/// <summary>
/// Reglas de inscripción de materias. Las consultan la capa de Aplicación y <see cref="Estudiante.Inscribir"/>.
/// </summary>
public static class PoliticaInscripcion
{
    /// <summary>Máximo de materias que puede inscribir un estudiante (R5).</summary>
    public const int MaximoMaterias = 3;

    /// <summary>Máximo de créditos del programa: tres materias de tres créditos (R2, R4).</summary>
    public const int MaximoCreditos = MaximoMaterias * PoliticaMateria.CreditosPorMateria;

    /// <summary>
    /// Evalúa si el estudiante puede inscribir la materia. Revisa las reglas en orden y devuelve la primera que se incumple.
    /// </summary>
    /// <param name="estudiante">Estudiante con sus inscripciones vigentes y las materias de cada una.</param>
    /// <param name="materia">Materia a inscribir.</param>
    /// <returns>El motivo del rechazo, o <see langword="null"/> si la inscripción es válida.</returns>
    public static MotivoRechazoInscripcion? ObtenerMotivoRechazo(Estudiante estudiante, Materia materia)
    {
        ArgumentNullException.ThrowIfNull(estudiante);
        ArgumentNullException.ThrowIfNull(materia);
        List<Inscripcion> activas = estudiante.InscripcionesActivas.ToList();

        if (!materia.Activo)
        {
            return MotivoRechazoInscripcion.MateriaInactiva;
        }

        if (materia.ProgramaId != estudiante.ProgramaId)
        {
            return MotivoRechazoInscripcion.MateriaDeOtroPrograma;
        }

        if (activas.Any(inscripcion => inscripcion.MateriaId == materia.Id))
        {
            return MotivoRechazoInscripcion.MateriaYaInscrita;
        }

        if (activas.Count >= MaximoMaterias)
        {
            return MotivoRechazoInscripcion.MaximoMateriasAlcanzado;
        }

        if (estudiante.CreditosInscritos + materia.Creditos > MaximoCreditos)
        {
            return MotivoRechazoInscripcion.MaximoCreditosSuperado;
        }

        if (activas.Any(inscripcion => inscripcion.Materia.ProfesorId == materia.ProfesorId))
        {
            return MotivoRechazoInscripcion.ProfesorRepetido;
        }

        return null;
    }
}
