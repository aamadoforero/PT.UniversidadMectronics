/** Una condición de la política de contraseñas. */
export interface CondicionContrasena {
  descripcion: string
  cumple: (contrasena: string) => boolean
}
