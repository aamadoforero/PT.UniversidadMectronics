import DashboardOutlined from '@mui/icons-material/DashboardOutlined'
import AssignmentOutlined from '@mui/icons-material/AssignmentOutlined'
import ClassOutlined from '@mui/icons-material/ClassOutlined'
import GroupOutlined from '@mui/icons-material/GroupOutlined'
import MenuBookOutlined from '@mui/icons-material/MenuBookOutlined'
import PersonOutlined from '@mui/icons-material/PersonOutlined'
import FactCheckOutlined from '@mui/icons-material/FactCheckOutlined'
import { Rutas } from '../rutas'
import { CodigosPermiso } from '../sesion/CodigosPermiso'
import type { OpcionMenu } from './OpcionMenu'

/** Opciones del menú lateral. */
export const opcionesMenu: readonly OpcionMenu[] = [
  { texto: 'Inicio', ruta: Rutas.Inicio, icono: <DashboardOutlined /> },
  { texto: 'Mis inscripciones', ruta: Rutas.MisInscripciones, icono: <AssignmentOutlined />, permiso: CodigosPermiso.InscripcionesPropias },
  { texto: 'Mis clases', ruta: Rutas.MisClases, icono: <ClassOutlined />, permiso: CodigosPermiso.MateriasDictadas },
  { texto: 'Todas las inscripciones', ruta: Rutas.TodasInscripciones, icono: <FactCheckOutlined />, permiso: CodigosPermiso.InscripcionesConsultar },
  { texto: 'Materias', ruta: Rutas.Materias, icono: <MenuBookOutlined />, permiso: CodigosPermiso.MateriasGestionar },
  { texto: 'Mi perfil', ruta: Rutas.MiPerfil, icono: <PersonOutlined /> },
  { texto: 'Usuarios', ruta: Rutas.Usuarios, icono: <GroupOutlined />, permiso: CodigosPermiso.UsuariosConsultar },
]
