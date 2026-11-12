import type { ReactNode } from 'react'

/** Opción del menú lateral; si tiene permiso, solo la ve quien lo tenga. */
export interface OpcionMenu {
  texto: string
  ruta: string
  icono: ReactNode
  permiso?: string
}
