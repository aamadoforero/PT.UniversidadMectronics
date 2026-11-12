/**
 * Página de resultados de una consulta paginada.
 * @typeParam T Tipo de los elementos de la página.
 */
export interface Pagina<T> {
  elementos: T[]
  numeroPagina: number
  tamanoPagina: number
  totalElementos: number
  totalPaginas: number
}
