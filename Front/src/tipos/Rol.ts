import type { TipoPerfil } from './TipoPerfil'

/** Rol que se puede asignar a una cuenta. */
export interface Rol {
  id: number
  nombre: string
  tipoPerfil: TipoPerfil
}
