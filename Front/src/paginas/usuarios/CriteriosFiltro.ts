/** Criterios elegidos en los filtros del listado de cuentas. */
export interface CriteriosFiltro {
  texto: string
  rolId: number | null
  activo: boolean | null
}
