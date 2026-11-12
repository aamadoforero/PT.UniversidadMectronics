/** Datos para crear o actualizar una cuenta (el rol solo se envía al crear). */
export interface DatosUsuario {
  nombreCompleto: string
  identificacion: string
  email: string
  rolId?: number
  programaId: number | null
  ubicacionSemestral: number | null
  categoriaId: number | null
}
