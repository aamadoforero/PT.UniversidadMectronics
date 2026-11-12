const PATRON_IDENTIFICACION: RegExp = /^[\p{L}\p{N}-]+$/u
const PATRON_CORREO: RegExp = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

/**
 * Valida el nombre completo (mismas reglas de `NombreCompleto` en el backend).
 * @param valor Texto escrito.
 * @returns `true` si es válido, o el mensaje de error.
 */
export function validarNombreCompleto(valor: string): true | string {
  const texto: string = valor.trim()
  if (texto === '') {
    return 'El nombre completo es obligatorio.'
  }
  return texto.length <= 150 || 'El nombre completo admite máximo 150 caracteres.'
}

/**
 * Valida la identificación (mismas reglas de `Identificacion` en el backend).
 * @param valor Texto escrito.
 * @returns `true` si es válida, o el mensaje de error.
 */
export function validarIdentificacion(valor: string): true | string {
  const texto: string = valor.trim()
  if (texto === '') {
    return 'La identificación es obligatoria.'
  }
  return (texto.length <= 50 && PATRON_IDENTIFICACION.test(texto))
    || 'La identificación admite letras, números y guiones, y máximo 50 caracteres.'
}

/**
 * Valida el correo (mismas reglas de `CorreoElectronico` en el backend).
 * @param valor Texto escrito.
 * @returns `true` si es válido, o el mensaje de error.
 */
export function validarCorreo(valor: string): true | string {
  const texto: string = valor.trim()
  if (texto === '') {
    return 'El correo es obligatorio.'
  }
  return (texto.length <= 150 && PATRON_CORREO.test(texto)) || 'El correo no tiene un formato válido.'
}
