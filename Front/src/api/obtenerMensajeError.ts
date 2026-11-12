import { ErrorApi } from './ErrorApi'

/**
 * Obtiene el mensaje que se muestra al usuario para cualquier error capturado.
 * @param error Error capturado en un bloque `catch`.
 * @returns Mensaje legible en español.
 */
export function obtenerMensajeError(error: unknown): string {
  if (error instanceof ErrorApi) {
    return error.message
  }
  return 'Ocurrió un error inesperado. Intente de nuevo más tarde.'
}
