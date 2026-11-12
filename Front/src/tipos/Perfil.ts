import type { PerfilEstudiante } from './PerfilEstudiante'
import type { PerfilProfesor } from './PerfilProfesor'
import type { TipoPerfil } from './TipoPerfil'

/** Perfil de la cuenta que inició sesión, con sus permisos. */
export interface Perfil {
  id: number
  nombreCompleto: string
  identificacion: string
  email: string
  rol: string
  tipoPerfil: TipoPerfil
  permisos: string[]
  debeCambiarContrasena: boolean
  perfilEstudiante: PerfilEstudiante | null
  perfilProfesor: PerfilProfesor | null
}
