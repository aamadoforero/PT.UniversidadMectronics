import type { ReactNode } from 'react'
import { Navigate, Outlet, useLocation, type Location } from 'react-router'
import { CargandoPagina } from '../componentes/CargandoPagina'
import { PaginaSinPermiso } from '../componentes/PaginaSinPermiso'
import { Rutas } from '../rutas'
import { useSesion } from './useSesion'

/** Propiedades de la ruta protegida. */
interface PropiedadesRutaProtegida {
  permiso?: string
}

/**
 * Deja pasar solo si hay sesión, si la contraseña temporal ya se cambió y, cuando se indica, si el usuario tiene el permiso.
 * @param propiedades Permiso requerido (opcional).
 */
export function RutaProtegida({ permiso }: PropiedadesRutaProtegida): ReactNode {
  const { perfil, cargando, tienePermiso } = useSesion()
  const ubicacion: Location = useLocation()

  if (cargando) {
    return <CargandoPagina />
  }

  if (perfil === null) {
    return <Navigate to={Rutas.InicioSesion} replace state={{ desde: ubicacion.pathname }} />
  }

  if (perfil.debeCambiarContrasena && ubicacion.pathname !== Rutas.CambiarContrasena) {
    return <Navigate to={Rutas.CambiarContrasena} replace />
  }

  if (permiso !== undefined && !tienePermiso(permiso)) {
    return <PaginaSinPermiso />
  }

  return <Outlet />
}
