import type { CondicionContrasena } from './CondicionContrasena'

/** Longitud mínima de la contraseña (igual a `PoliticaContrasena.LongitudMinima`). */
export const LONGITUD_MINIMA_CONTRASENA: number = 8

/** Longitud máxima de la contraseña (igual a `PoliticaContrasena.LongitudMaxima`). */
export const LONGITUD_MAXIMA_CONTRASENA: number = 128

/** Condiciones de la política de contraseñas, con los mismos textos del backend. */
export const condicionesContrasena: readonly CondicionContrasena[] = [
  { descripcion: `Debe tener al menos ${LONGITUD_MINIMA_CONTRASENA} caracteres.`, cumple: (c: string): boolean => c.length >= LONGITUD_MINIMA_CONTRASENA },
  { descripcion: `Debe tener como máximo ${LONGITUD_MAXIMA_CONTRASENA} caracteres.`, cumple: (c: string): boolean => c.length <= LONGITUD_MAXIMA_CONTRASENA },
  { descripcion: 'Debe incluir al menos una letra mayúscula.', cumple: (c: string): boolean => /\p{Lu}/u.test(c) },
  { descripcion: 'Debe incluir al menos una letra minúscula.', cumple: (c: string): boolean => /\p{Ll}/u.test(c) },
  { descripcion: 'Debe incluir al menos un número.', cumple: (c: string): boolean => /\p{Nd}/u.test(c) },
  { descripcion: 'Debe incluir al menos un símbolo.', cumple: (c: string): boolean => /[^\p{L}\p{N}\s\p{Cc}]/u.test(c) },
  { descripcion: 'No puede contener espacios.', cumple: (c: string): boolean => !/\s/.test(c) },
]

/**
 * Valida una contraseña contra la política.
 * @param contrasena Contraseña escrita por el usuario.
 * @returns `true` si cumple, o el primer incumplimiento.
 */
export function validarContrasena(contrasena: string): true | string {
  const incumplida: CondicionContrasena | undefined = condicionesContrasena.find(
    (condicion: CondicionContrasena) => !condicion.cumple(contrasena),
  )
  return incumplida === undefined ? true : incumplida.descripcion
}
