import type { PerfilEstudiante } from './PerfilEstudiante'
import type { PerfilProfesor } from './PerfilProfesor'
import type { TipoPerfil } from './TipoPerfil'

/** Detalle completo de una cuenta de usuario. */
export interface UsuarioDetalle {
  id: number
  nombreCompleto: string
  identificacion: string
  email: string
  rolId: number
  rol: string
  tipoPerfil: TipoPerfil
  activo: boolean
  debeCambiarContrasena: boolean
  estaBloqueado: boolean
  bloqueoHasta: string | null
  fechaCreacion: string
  fechaModificacion: string | null
  perfilEstudiante: PerfilEstudiante | null
  perfilProfesor: PerfilProfesor | null
}
