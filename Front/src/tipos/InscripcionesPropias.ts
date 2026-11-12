import type { Inscripcion } from './Inscripcion'

/** Inscripciones del estudiante con sus totales y los límites del programa de créditos. */
export interface InscripcionesPropias {
  materias: Inscripcion[]
  creditosInscritos: number
  creditosMaximos: number
  materiasMaximas: number
}
