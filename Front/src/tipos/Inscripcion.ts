/** Materia inscrita por el estudiante. */
export interface Inscripcion {
  materiaId: number
  materia: string
  creditos: number
  profesorId: number
  profesor: string
  fechaInscripcion: string
}
