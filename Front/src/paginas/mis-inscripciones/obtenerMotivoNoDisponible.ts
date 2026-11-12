import type { Inscripcion } from '../../tipos/Inscripcion'
import type { InscripcionesPropias } from '../../tipos/InscripcionesPropias'
import type { Materia } from '../../tipos/Materia'

/**
 * Indica por qué el estudiante no puede inscribir una materia, con las mismas reglas de `PoliticaInscripcion` del backend
 * (máximo de materias, máximo de créditos y no repetir profesor). El backend vuelve a validar siempre.
 * @param materia Materia que se quiere inscribir.
 * @param inscripciones Inscripciones actuales del estudiante, cada una con el identificador de su profesor.
 * @returns El motivo, o `null` si la materia se puede inscribir.
 */
export function obtenerMotivoNoDisponible(materia: Materia, inscripciones: InscripcionesPropias): string | null {
  if (inscripciones.materias.length >= inscripciones.materiasMaximas) {
    return `Ya tiene ${inscripciones.materiasMaximas} materias, el máximo permitido.`
  }

  if (inscripciones.creditosInscritos + materia.creditos > inscripciones.creditosMaximos) {
    return `Superaría el máximo de ${inscripciones.creditosMaximos} créditos.`
  }

  const repiteProfesor: boolean = inscripciones.materias.some(
    (inscripcion: Inscripcion) => inscripcion.profesorId === materia.profesorId,
  )
  if (repiteProfesor) {
    return `Ya tiene una clase con ${materia.profesor}.`
  }

  return null
}
