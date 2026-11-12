import type { UsuarioSesion } from './UsuarioSesion'

/** Respuesta del backend al iniciar, renovar la sesión o cambiar la contraseña. */
export interface RespuestaSesion {
  usuario: UsuarioSesion
  venceAccesoUtc: string
  venceRenovacionUtc: string
}
