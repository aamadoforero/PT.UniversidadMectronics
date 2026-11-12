import type { Perfil } from '../tipos/Perfil'

/** Lo que el contexto de sesión ofrece a cualquier componente. */
export interface ValorContextoSesion {
  perfil: Perfil | null
  cargando: boolean
  iniciarSesion: (email: string, contrasena: string) => Promise<void>
  cerrarSesion: () => Promise<void>
  recargarPerfil: () => Promise<void>
  tienePermiso: (codigo: string) => boolean
}
