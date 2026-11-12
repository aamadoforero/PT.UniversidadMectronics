/** Datos básicos del usuario devueltos al crear o renovar la sesión. */
export interface UsuarioSesion {
  id: number
  nombreCompleto: string
  email: string
  rol: string
  permisos: string[]
  debeCambiarContrasena: boolean
}
