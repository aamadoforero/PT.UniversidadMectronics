/** Criterios de búsqueda del listado de cuentas. */
export interface FiltroUsuarios {
  texto: string
  rolId: number | null
  activo: boolean | null
  numeroPagina: number
  tamanoPagina: number
}
