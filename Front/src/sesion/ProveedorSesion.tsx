import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'
import { registrarManejadorSesionExpirada } from '../api/clienteHttp'
import * as sesionApi from '../api/sesionApi'
import type { Perfil } from '../tipos/Perfil'
import { ContextoSesion } from './ContextoSesion'
import type { ValorContextoSesion } from './ValorContextoSesion'

/** Propiedades del proveedor de sesión. */
interface PropiedadesProveedorSesion {
  children: ReactNode
}

/**
 * Carga el perfil al abrir la aplicación y comparte la sesión con los componentes hijos.
 * @param propiedades Componentes que tendrán acceso a la sesión.
 */
export function ProveedorSesion({ children }: PropiedadesProveedorSesion): ReactNode {
  const [perfil, setPerfil] = useState<Perfil | null>(null)
  const [cargando, setCargando] = useState<boolean>(true)

  const recargarPerfil = useCallback(async (): Promise<void> => {
    try {
      setPerfil(await sesionApi.obtenerPerfil())
    } catch {
      setPerfil(null)
    }
  }, [])

  useEffect(() => {
    registrarManejadorSesionExpirada(() => setPerfil(null))
    sesionApi.obtenerPerfil()
      .then(setPerfil)
      .catch(() => setPerfil(null))
      .finally(() => setCargando(false))
  }, [])

  const iniciarSesion = useCallback(async (email: string, contrasena: string): Promise<void> => {
    await sesionApi.iniciarSesion(email, contrasena)
    setPerfil(await sesionApi.obtenerPerfil())
  }, [])

  const cerrarSesion = useCallback(async (): Promise<void> => {
    try {
      await sesionApi.cerrarSesion()
    } finally {
      setPerfil(null)
    }
  }, [])

  const tienePermiso = useCallback(
    (codigo: string): boolean => perfil?.permisos.includes(codigo) ?? false,
    [perfil],
  )

  const valor: ValorContextoSesion = useMemo(
    () => ({ perfil, cargando, iniciarSesion, cerrarSesion, recargarPerfil, tienePermiso }),
    [perfil, cargando, iniciarSesion, cerrarSesion, recargarPerfil, tienePermiso],
  )

  return <ContextoSesion.Provider value={valor}>{children}</ContextoSesion.Provider>
}
