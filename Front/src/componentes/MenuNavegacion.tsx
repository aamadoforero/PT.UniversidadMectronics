import { List, ListItemButton, ListItemIcon, ListItemText } from '@mui/material'
import type { ReactNode } from 'react'
import { NavLink } from 'react-router'
import { useSesion } from '../sesion/useSesion'
import type { OpcionMenu } from './OpcionMenu'
import { opcionesMenu } from './opcionesMenu'

/** Propiedades del menú de navegación. */
interface PropiedadesMenuNavegacion {
  alSeleccionar: () => void
}

/**
 * Lista de opciones del menú, filtrada por los permisos del usuario.
 * @param propiedades Función que se ejecuta al elegir una opción (cierra el menú en móviles).
 */
export function MenuNavegacion({ alSeleccionar }: PropiedadesMenuNavegacion): ReactNode {
  const { tienePermiso } = useSesion()
  const visibles: OpcionMenu[] = opcionesMenu.filter(
    (opcion: OpcionMenu) => opcion.permiso === undefined || tienePermiso(opcion.permiso),
  )

  return (
    <List sx={{ px: 1 }}>
      {visibles.map((opcion: OpcionMenu) => (
        <ListItemButton
          key={opcion.ruta}
          component={NavLink}
          to={opcion.ruta}
          end={opcion.ruta === '/'}
          onClick={alSeleccionar}
          sx={{ borderRadius: 2, mb: 0.5, '&.active': { bgcolor: 'primary.main', color: 'primary.contrastText', '& .MuiListItemIcon-root': { color: 'inherit' } } }}
        >
          <ListItemIcon sx={{ minWidth: 40 }}>{opcion.icono}</ListItemIcon>
          <ListItemText primary={opcion.texto} />
        </ListItemButton>
      ))}
    </List>
  )
}
