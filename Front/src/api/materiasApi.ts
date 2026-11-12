import type { DatosMateria } from '../tipos/DatosMateria'
import type { Materia } from '../tipos/Materia'
import type { MateriaDictada } from '../tipos/MateriaDictada'
import { consultar, enviar } from './clienteHttp'

const RUTA_MATERIAS: string = '/api/academico/materias'

/**
 * Obtiene las materias del catálogo.
 * @param incluirInactivas `true` para incluir las desactivadas (exige `materias.gestionar`).
 * @returns Lista de materias.
 */
export function obtenerMaterias(incluirInactivas: boolean): Promise<Materia[]> {
  return consultar<Materia[]>(`${RUTA_MATERIAS}?incluirInactivas=${incluirInactivas}`)
}

/**
 * Crea una materia.
 * @param datos Nombre, créditos, programa y profesor.
 * @returns Materia creada.
 */
export function crearMateria(datos: DatosMateria): Promise<Materia> {
  return enviar<Materia>('POST', RUTA_MATERIAS, datos)
}

/**
 * Actualiza el nombre, los créditos o el profesor de una materia.
 * @param id Identificador de la materia.
 * @param datos Datos nuevos.
 * @returns Materia actualizada.
 */
export function actualizarMateria(id: number, datos: DatosMateria): Promise<Materia> {
  return enviar<Materia>('PUT', `${RUTA_MATERIAS}/${id}`, datos)
}

/**
 * Activa o desactiva una materia.
 * @param id Identificador de la materia.
 * @param activo `true` para activarla, `false` para desactivarla.
 * @returns Materia actualizada.
 */
export function cambiarEstadoMateria(id: number, activo: boolean): Promise<Materia> {
  return enviar<Materia>('PUT', `${RUTA_MATERIAS}/${id}/estado`, { activo })
}

/**
 * Obtiene las materias que dicta el profesor que inició sesión.
 * @returns Materias con sus estudiantes inscritos.
 */
export function obtenerMateriasDictadas(): Promise<MateriaDictada[]> {
  return consultar<MateriaDictada[]>(`${RUTA_MATERIAS}/dictadas`)
}
