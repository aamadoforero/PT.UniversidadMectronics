/** Contraseña temporal generada al restablecer el acceso de una cuenta. */
export interface ContrasenaRestablecida {
  usuarioId: number
  contrasenaTemporal: string
}
