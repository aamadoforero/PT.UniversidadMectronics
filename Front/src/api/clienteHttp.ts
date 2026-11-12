import type { ProblemaApi } from '../tipos/ProblemaApi'
import { ErrorApi } from './ErrorApi'

const RUTA_SESION: string = '/api/seguridad/sesion'
const RUTA_RENOVACION: string = '/api/seguridad/sesion/renovacion'
const NOMBRE_COOKIE_CSRF: string = 'universidad-csrf'
const CABECERA_CSRF: string = 'X-CSRF-TOKEN'
const MENSAJE_SIN_CONEXION: string = 'No fue posible conectar con el servidor. Intente de nuevo en unos segundos.'

let renovacionEnCurso: Promise<boolean> | null = null
let manejadorSesionExpirada: (() => void) | null = null

/**
 * Registra la función que se ejecuta cuando la sesión venció y no se pudo renovar.
 * @param manejador Función que limpia la sesión en la interfaz.
 */
export function registrarManejadorSesionExpirada(manejador: () => void): void {
  manejadorSesionExpirada = manejador
}

/**
 * Consulta un recurso con GET.
 * @typeParam T Tipo de la respuesta.
 * @param ruta Ruta relativa del endpoint, p. ej. `/api/seguridad/cuenta/perfil`.
 * @returns El cuerpo de la respuesta.
 */
export function consultar<T>(ruta: string): Promise<T> {
  return enviarSolicitud<T>('GET', ruta, undefined)
}

/**
 * Envía una operación que modifica datos (POST, PUT o DELETE) con el token CSRF.
 * @typeParam T Tipo de la respuesta (`void` si el backend responde 204).
 * @param metodo Método HTTP.
 * @param ruta Ruta relativa del endpoint.
 * @param cuerpo Objeto que se envía como JSON (opcional).
 * @returns El cuerpo de la respuesta.
 */
export function enviar<T>(metodo: 'POST' | 'PUT' | 'DELETE', ruta: string, cuerpo?: unknown): Promise<T> {
  return enviarSolicitud<T>(metodo, ruta, cuerpo)
}

async function enviarSolicitud<T>(metodo: string, ruta: string, cuerpo: unknown): Promise<T> {
  let respuesta: Response = await ejecutarPeticion(metodo, ruta, cuerpo)

  if (respuesta.status === 401 && !ruta.startsWith(RUTA_SESION)) {
    const renovada: boolean = await renovarSesion()
    if (!renovada) {
      manejadorSesionExpirada?.()
      throw await convertirAError(respuesta)
    }
    respuesta = await ejecutarPeticion(metodo, ruta, cuerpo)
  }

  if (!respuesta.ok) {
    throw await convertirAError(respuesta)
  }

  if (respuesta.status === 204) {
    return undefined as T
  }

  return (await respuesta.json()) as T
}

async function ejecutarPeticion(metodo: string, ruta: string, cuerpo: unknown): Promise<Response> {
  const cabeceras: Headers = new Headers({ Accept: 'application/json' })
  if (cuerpo !== undefined) {
    cabeceras.set('Content-Type', 'application/json')
  }

  const tokenCsrf: string | null = leerTokenCsrf()
  if (metodo !== 'GET' && tokenCsrf !== null) {
    cabeceras.set(CABECERA_CSRF, tokenCsrf)
  }

  try {
    return await fetch(ruta, {
      method: metodo,
      headers: cabeceras,
      body: cuerpo === undefined ? undefined : JSON.stringify(cuerpo),
      credentials: 'same-origin',
    })
  } catch {
    throw new ErrorApi(0, 'Conexion.Fallida', MENSAJE_SIN_CONEXION, {})
  }
}

function renovarSesion(): Promise<boolean> {
  renovacionEnCurso ??= solicitarRenovacion().finally(() => {
    renovacionEnCurso = null
  })
  return renovacionEnCurso
}

async function solicitarRenovacion(): Promise<boolean> {
  if (leerTokenCsrf() === null) {
    return false
  }

  try {
    const respuesta: Response = await ejecutarPeticion('POST', RUTA_RENOVACION, undefined)
    return respuesta.ok
  } catch {
    return false
  }
}

function leerTokenCsrf(): string | null {
  const prefijo: string = `${NOMBRE_COOKIE_CSRF}=`
  const cookie: string | undefined = document.cookie
    .split('; ')
    .find((elemento: string) => elemento.startsWith(prefijo))
  return cookie === undefined ? null : decodeURIComponent(cookie.substring(prefijo.length))
}

async function convertirAError(respuesta: Response): Promise<ErrorApi> {
  const problema: ProblemaApi = await leerProblema(respuesta)
  return new ErrorApi(
    respuesta.status,
    problema.codigo ?? `Http.${respuesta.status}`,
    problema.title ?? obtenerMensajePorEstado(respuesta.status),
    problema.errores ?? {},
  )
}

async function leerProblema(respuesta: Response): Promise<ProblemaApi> {
  const tipoContenido: string = respuesta.headers.get('Content-Type') ?? ''
  if (!tipoContenido.includes('json')) {
    return {}
  }

  try {
    return (await respuesta.json()) as ProblemaApi
  } catch {
    return {}
  }
}

function obtenerMensajePorEstado(estado: number): string {
  if (estado === 401) {
    return 'Su sesión expiró. Inicie sesión nuevamente.'
  }
  if (estado === 403) {
    return 'No tiene permiso para realizar esta operación.'
  }
  if (estado === 404) {
    return 'El recurso solicitado no existe.'
  }
  if (estado >= 500) {
    return 'Ocurrió un error inesperado. Intente de nuevo más tarde.'
  }
  return 'No fue posible completar la operación.'
}
