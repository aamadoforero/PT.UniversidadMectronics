import type { CompanerosClase } from '../tipos/CompanerosClase'
import type { Inscripcion } from '../tipos/Inscripcion'
import type { InscripcionesPropias } from '../tipos/InscripcionesPropias'
import { consultar, enviar } from './clienteHttp'

const RUTA_PROPIAS: string = '/api/academico/inscripciones/propias'

/**
 * Obtiene las materias inscritas por el estudiante y sus créditos.
 * @returns Inscripciones y límites del programa.
 */
export function obtenerInscripcionesPropias(): Promise<InscripcionesPropias> {
  return consultar<InscripcionesPropias>(RUTA_PROPIAS)
}

/**
 * Inscribe una materia; el backend aplica las reglas de máximo de materias, créditos y profesor repetido.
 * @param materiaId Materia que se inscribe.
 * @returns Inscripción creada.
 */
export function inscribirMateria(materiaId: number): Promise<Inscripcion> {
  return enviar<Inscripcion>('POST', RUTA_PROPIAS, { materiaId })
}

/**
 * Cancela la inscripción de una materia.
 * @param materiaId Materia que se cancela.
 */
export function cancelarInscripcion(materiaId: number): Promise<void> {
  return enviar<void>('DELETE', `${RUTA_PROPIAS}/${materiaId}`)
}

/**
 * Obtiene los nombres de los compañeros de una materia inscrita.
 * @param materiaId Materia inscrita.
 * @returns Nombres de los compañeros.
 */
export function obtenerCompaneros(materiaId: number): Promise<CompanerosClase> {
  return consultar<CompanerosClase>(`${RUTA_PROPIAS}/${materiaId}/companeros`)
}
