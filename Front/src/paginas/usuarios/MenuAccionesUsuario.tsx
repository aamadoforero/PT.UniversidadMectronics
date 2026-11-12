import MoreVert from '@mui/icons-material/MoreVert'
import { IconButton, Menu, MenuItem } from '@mui/material'
import { useState, type MouseEvent, type ReactNode } from 'react'
import { CodigosPermiso } from '../../sesion/CodigosPermiso'
import { useSesion } from '../../sesion/useSesion'
import type { UsuarioResumen } from '../../tipos/UsuarioResumen'
import type { AccionUsuario } from './AccionUsuario'

/** Propiedades del menú de acciones. */
interface PropiedadesMenuAcciones {
  usuario: UsuarioResumen
  alElegir: (accion: AccionUsuario, usuario: UsuarioResumen) => void
}

/**
 * Menú de acciones de una fila; muestra solo las que el usuario actual tiene permitidas y nunca sobre su propia cuenta.
 * @param propiedades Cuenta de la fila y función que recibe la acción elegida.
 */
export function MenuAccionesUsuario({ usuario, alElegir }: PropiedadesMenuAcciones): ReactNode {
  const { perfil, tienePermiso } = useSesion()
  const [ancla, setAncla] = useState<HTMLElement | null>(null)

  if (perfil?.id === usuario.id) {
    return null
  }

  function abrirMenu(evento: MouseEvent<HTMLElement>): void {
    setAncla(evento.currentTarget)
  }

  function elegir(accion: AccionUsuario): void {
    setAncla(null)
    alElegir(accion, usuario)
  }

  return (
    <>
      <IconButton onClick={abrirMenu} aria-label={`Acciones de ${usuario.nombreCompleto}`}>
        <MoreVert />
      </IconButton>
      <Menu anchorEl={ancla} open={ancla !== null} onClose={() => setAncla(null)}>
        {tienePermiso(CodigosPermiso.UsuariosEditar) && <MenuItem onClick={() => elegir('editar')}>Editar datos</MenuItem>}
        {tienePermiso(CodigosPermiso.UsuariosCambiarEstado) && (
          <MenuItem onClick={() => elegir('cambiarEstado')}>{usuario.activo ? 'Desactivar cuenta' : 'Activar cuenta'}</MenuItem>
        )}
        {tienePermiso(CodigosPermiso.UsuariosRestablecerContrasena) && (
          <MenuItem onClick={() => elegir('restablecerContrasena')}>Restablecer contraseña</MenuItem>
        )}
      </Menu>
    </>
  )
}
