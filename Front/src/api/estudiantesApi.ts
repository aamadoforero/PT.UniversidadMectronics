import type { EstudianteResumen } from '../tipos/EstudianteResumen'
import type { FiltroEstudiantes } from '../tipos/FiltroEstudiantes'
import type { Pagina } from '../tipos/Pagina'
import { consultar } from './clienteHttp'

/**
 * Busca los registros de estudiantes con paginación.
 * @param filtro Texto de búsqueda y página solicitada.
 * @returns Página de resultados.
 */
export function buscarEstudiantes(filtro: FiltroEstudiantes): Promise<Pagina<EstudianteResumen>> {
  const parametros: URLSearchParams = new URLSearchParams({
    numeroPagina: String(filtro.numeroPagina),
    tamanoPagina: String(filtro.tamanoPagina),
  })
  if (filtro.texto.trim() !== '') {
    parametros.set('texto', filtro.texto.trim())
  }
  return consultar<Pagina<EstudianteResumen>>(`/api/academico/estudiantes?${parametros.toString()}`)
}
