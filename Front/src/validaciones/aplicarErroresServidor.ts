import type { FieldValues, Path, UseFormSetError } from 'react-hook-form'
import { ErrorApi } from '../api/ErrorApi'

/**
 * Muestra bajo cada campo del formulario los errores de validación que devolvió el backend.
 * @typeParam T Tipo de los valores del formulario.
 * @param error Error capturado al enviar el formulario.
 * @param asignarError Función `setError` de React Hook Form.
 */
export function aplicarErroresServidor<T extends FieldValues>(error: unknown, asignarError: UseFormSetError<T>): void {
  if (!(error instanceof ErrorApi)) {
    return
  }

  for (const [campo, mensajes] of Object.entries(error.errores)) {
    const nombreCampo: string = campo.charAt(0).toLowerCase() + campo.slice(1)
    asignarError(nombreCampo as Path<T>, { type: 'servidor', message: mensajes.join(' ') })
  }
}
