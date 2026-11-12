import type { Perfil } from '../tipos/Perfil'
import type { RespuestaSesion } from '../tipos/RespuestaSesion'
import { consultar, enviar } from './clienteHttp'

/**
 * Inicia sesión; el backend deja el token en cookies HttpOnly.
 * @param email Correo de la cuenta.
 * @param contrasena Contraseña de la cuenta.
 * @returns Datos de la sesión creada.
 */
export function iniciarSesion(email: string, contrasena: string): Promise<RespuestaSesion> {
  return enviar<RespuestaSesion>('POST', '/api/seguridad/sesion', { email, contrasena })
}

/** Cierra la sesión y elimina las cookies. */
export function cerrarSesion(): Promise<void> {
  return enviar<void>('DELETE', '/api/seguridad/sesion')
}

/**
 * Obtiene el perfil de la cuenta que inició sesión.
 * @returns Perfil con permisos.
 */
export function obtenerPerfil(): Promise<Perfil> {
  return consultar<Perfil>('/api/seguridad/cuenta/perfil')
}

/**
 * Cambia la contraseña de la cuenta propia; el backend renueva la sesión.
 * @param contrasenaActual Contraseña vigente (o temporal).
 * @param contrasenaNueva Contraseña nueva.
 * @returns Datos de la nueva sesión.
 */
export function cambiarContrasena(contrasenaActual: string, contrasenaNueva: string): Promise<RespuestaSesion> {
  return enviar<RespuestaSesion>('PUT', '/api/seguridad/cuenta/contrasena', { contrasenaActual, contrasenaNueva })
}
