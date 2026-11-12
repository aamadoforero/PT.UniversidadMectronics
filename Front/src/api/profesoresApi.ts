import type { Profesor } from '../tipos/Profesor'
import { consultar } from './clienteHttp'

/**
 * Obtiene los profesores con su cupo de materias.
 * @returns Lista de profesores.
 */
export function obtenerProfesores(): Promise<Profesor[]> {
  return consultar<Profesor[]>('/api/academico/profesores')
}
