/** Datos para crear o actualizar una materia (el programa solo se envía al crear; los créditos los fija el backend en 3). */
export interface DatosMateria {
  nombre: string
  profesorId: number
  programaId?: number
}
