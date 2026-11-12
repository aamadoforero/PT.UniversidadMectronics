/** Datos académicos de una cuenta de estudiante. */
export interface PerfilEstudiante {
  programaId: number
  /** Nombre del programa; lo entrega Seguridad para no consultar el catálogo. */
  nombrePrograma: string | null
  ubicacionSemestral: number
}
