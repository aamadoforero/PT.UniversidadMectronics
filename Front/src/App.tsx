import { lazy, Suspense, type LazyExoticComponent, type ReactNode } from 'react'
import { Route, Routes } from 'react-router'
import { CargandoPagina } from './componentes/CargandoPagina'
import { DisenoPrincipal } from './componentes/DisenoPrincipal'
import { Rutas } from './rutas'
import { CodigosPermiso } from './sesion/CodigosPermiso'
import { RutaProtegida } from './sesion/RutaProtegida'

const PaginaInicioSesion: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/inicio-sesion/PaginaInicioSesion'))
const PaginaInicio: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/inicio/PaginaInicio'))
const PaginaMiPerfil: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/mi-perfil/PaginaMiPerfil'))
const PaginaCambiarContrasena: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/cambiar-contrasena/PaginaCambiarContrasena'))
const PaginaUsuarios: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/usuarios/PaginaUsuarios'))
const PaginaMisInscripciones: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/mis-inscripciones/PaginaMisInscripciones'))
const PaginaTodasInscripciones: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/todas-inscripciones/PaginaTodasInscripciones'))
const PaginaMaterias: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/materias/PaginaMaterias'))
const PaginaMisClases: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/mis-clases/PaginaMisClases'))
const PaginaNoEncontrada: LazyExoticComponent<() => ReactNode> = lazy(() => import('./paginas/no-encontrada/PaginaNoEncontrada'))

/** Mapa de rutas de la aplicación. Cada página se descarga solo cuando se visita (carga diferida). */
export function App(): ReactNode {
  return (
    <Suspense fallback={<CargandoPagina />}>
      <Routes>
        <Route path={Rutas.InicioSesion} element={<PaginaInicioSesion />} />

        <Route element={<RutaProtegida />}>
          <Route element={<DisenoPrincipal />}>
            <Route path={Rutas.Inicio} element={<PaginaInicio />} />
            <Route path={Rutas.MiPerfil} element={<PaginaMiPerfil />} />
            <Route path={Rutas.CambiarContrasena} element={<PaginaCambiarContrasena />} />
            <Route element={<RutaProtegida permiso={CodigosPermiso.UsuariosConsultar} />}>
              <Route path={Rutas.Usuarios} element={<PaginaUsuarios />} />
            </Route>
            <Route element={<RutaProtegida permiso={CodigosPermiso.InscripcionesPropias} />}>
              <Route path={Rutas.MisInscripciones} element={<PaginaMisInscripciones />} />
            </Route>
            <Route element={<RutaProtegida permiso={CodigosPermiso.InscripcionesConsultar} />}>
              <Route path={Rutas.TodasInscripciones} element={<PaginaTodasInscripciones />} />
            </Route>
            <Route element={<RutaProtegida permiso={CodigosPermiso.MateriasGestionar} />}>
              <Route path={Rutas.Materias} element={<PaginaMaterias />} />
            </Route>
            <Route element={<RutaProtegida permiso={CodigosPermiso.MateriasDictadas} />}>
              <Route path={Rutas.MisClases} element={<PaginaMisClases />} />
            </Route>
          </Route>
        </Route>

        <Route path="*" element={<PaginaNoEncontrada />} />
      </Routes>
    </Suspense>
  )
}
