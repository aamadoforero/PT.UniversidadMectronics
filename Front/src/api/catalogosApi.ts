import type { ElementoCatalogo } from '../tipos/ElementoCatalogo'
import type { Rol } from '../tipos/Rol'
import { consultar } from './clienteHttp'

/**
 * Obtiene los roles que el usuario actual puede asignar.
 * @returns Lista de roles.
 */
export function obtenerRoles(): Promise<Rol[]> {
  return consultar<Rol[]>('/api/seguridad/catalogos/roles')
}

/**
 * Obtiene los programas académicos activos.
 * @returns Lista de programas.
 */
export function obtenerProgramas(): Promise<ElementoCatalogo[]> {
  return consultar<ElementoCatalogo[]>('/api/seguridad/catalogos/programas')
}

/**
 * Obtiene las categorías de vinculación de profesor.
 * @returns Lista de categorías.
 */
export function obtenerCategoriasProfesor(): Promise<ElementoCatalogo[]> {
  return consultar<ElementoCatalogo[]>('/api/seguridad/catalogos/categorias-profesor')
}
