/** Formato Problem Details (RFC 9457) que devuelve el backend ante un error. */
export interface ProblemaApi {
  title?: string
  status?: number
  codigo?: string
  errores?: Record<string, string[]>
  traceId?: string
}
