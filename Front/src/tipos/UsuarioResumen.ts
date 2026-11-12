/** Fila del listado de cuentas de usuario. */
export interface UsuarioResumen {
  id: number
  nombreCompleto: string
  identificacion: string
  email: string
  rol: string
  activo: boolean
  estaBloqueado: boolean
  debeCambiarContrasena: boolean
}
