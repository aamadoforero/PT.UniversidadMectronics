import type { UsuarioDetalle } from './UsuarioDetalle'

/** Cuenta recién creada junto con su contraseña temporal, que solo se muestra una vez. */
export interface UsuarioCreado {
  usuario: UsuarioDetalle
  contrasenaTemporal: string
}
