/** Error devuelto por el backend (o por la red), ya traducido para la interfaz. */
export class ErrorApi extends Error {
  readonly estado: number
  readonly codigo: string
  readonly errores: Readonly<Record<string, string[]>>

  /**
   * Crea el error a partir de la respuesta del backend.
   * @param estado Código de estado HTTP (0 si no hubo conexión).
   * @param codigo Código de error del backend, p. ej. `Usuario.EmailDuplicado`.
   * @param mensaje Mensaje legible para el usuario.
   * @param errores Errores por campo, con el nombre del campo como clave.
   */
  constructor(estado: number, codigo: string, mensaje: string, errores: Record<string, string[]>) {
    super(mensaje)
    this.name = 'ErrorApi'
    this.estado = estado
    this.codigo = codigo
    this.errores = errores
  }
}
