/** Materia del catálogo académico. */
export interface Materia {
  id: number
  nombre: string
  creditos: number
  programaId: number
  profesorId: number
  profesor: string
  activo: boolean
}
