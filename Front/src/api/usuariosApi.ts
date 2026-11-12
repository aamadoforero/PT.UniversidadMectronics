import type { ContrasenaRestablecida } from '../tipos/ContrasenaRestablecida'
import type { DatosUsuario } from '../tipos/DatosUsuario'
import type { FiltroUsuarios } from '../tipos/FiltroUsuarios'
import type { Pagina } from '../tipos/Pagina'
import type { UsuarioCreado } from '../tipos/UsuarioCreado'
import type { UsuarioDetalle } from '../tipos/UsuarioDetalle'
import type { UsuarioResumen } from '../tipos/UsuarioResumen'
import { consultar, enviar } from './clienteHttp'

const RUTA_USUARIOS: string = '/api/seguridad/usuarios'

/**
 * Busca cuentas de usuario con paginación.
 * @param filtro Criterios de búsqueda y página solicitada.
 * @returns Página de resultados.
 */
export function buscarUsuarios(filtro: FiltroUsuarios): Promise<Pagina<UsuarioResumen>> {
  const parametros: URLSearchParams = new URLSearchParams({
    numeroPagina: String(filtro.numeroPagina),
    tamanoPagina: String(filtro.tamanoPagina),
  })
  if (filtro.texto.trim() !== '') {
    parametros.set('texto', filtro.texto.trim())
  }
  if (filtro.rolId !== null) {
    parametros.set('rolId', String(filtro.rolId))
  }
  if (filtro.activo !== null) {
    parametros.set('activo', String(filtro.activo))
  }
  return consultar<Pagina<UsuarioResumen>>(`${RUTA_USUARIOS}?${parametros.toString()}`)
}

/**
 * Obtiene el detalle de una cuenta.
 * @param id Identificador de la cuenta.
 * @returns Detalle de la cuenta.
 */
export function obtenerUsuario(id: number): Promise<UsuarioDetalle> {
  return consultar<UsuarioDetalle>(`${RUTA_USUARIOS}/${id}`)
}

/**
 * Crea una cuenta; el backend genera una contraseña temporal.
 * @param datos Datos de la cuenta, incluido el rol.
 * @returns Cuenta creada y su contraseña temporal.
 */
export function crearUsuario(datos: DatosUsuario): Promise<UsuarioCreado> {
  return enviar<UsuarioCreado>('POST', RUTA_USUARIOS, datos)
}

/**
 * Actualiza los datos de una cuenta.
 * @param id Identificador de la cuenta.
 * @param datos Datos nuevos.
 * @returns Cuenta actualizada.
 */
export function actualizarUsuario(id: number, datos: DatosUsuario): Promise<UsuarioDetalle> {
  return enviar<UsuarioDetalle>('PUT', `${RUTA_USUARIOS}/${id}`, datos)
}

/**
 * Activa o desactiva una cuenta.
 * @param id Identificador de la cuenta.
 * @param activo `true` para activarla, `false` para desactivarla.
 * @returns Cuenta actualizada.
 */
export function cambiarEstadoUsuario(id: number, activo: boolean): Promise<UsuarioDetalle> {
  return enviar<UsuarioDetalle>('PUT', `${RUTA_USUARIOS}/${id}/estado`, { activo })
}

/**
 * Genera una contraseña temporal nueva para una cuenta.
 * @param id Identificador de la cuenta.
 * @returns Contraseña temporal generada.
 */
export function restablecerContrasena(id: number): Promise<ContrasenaRestablecida> {
  return enviar<ContrasenaRestablecida>('POST', `${RUTA_USUARIOS}/${id}/contrasena-temporal`)
}
