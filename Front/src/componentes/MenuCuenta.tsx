import AccountCircle from '@mui/icons-material/AccountCircle'
import { Box, Divider, IconButton, ListItemText, Menu, MenuItem, Tooltip, Typography } from '@mui/material'
import { useState, type MouseEvent, type ReactNode } from 'react'
import { useNavigate, type NavigateFunction } from 'react-router'
import { Rutas } from '../rutas'
import { useSesion } from '../sesion/useSesion'

/** Botón de la barra superior con los datos de la cuenta, cambio de contraseña y cierre de sesión. */
export function MenuCuenta(): ReactNode {
  const { perfil, cerrarSesion } = useSesion()
  const navegar: NavigateFunction = useNavigate()
  const [ancla, setAncla] = useState<HTMLElement | null>(null)

  function abrirMenu(evento: MouseEvent<HTMLElement>): void {
    setAncla(evento.currentTarget)
  }

  function irA(ruta: string): void {
    setAncla(null)
    navegar(ruta)
  }

  async function salir(): Promise<void> {
    setAncla(null)
    await cerrarSesion()
    navegar(Rutas.InicioSesion, { replace: true })
  }

  return (
    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
      <Box sx={{ textAlign: 'right', display: { xs: 'none', sm: 'block' } }}>
        <Typography variant="body2" sx={{ fontWeight: 600, lineHeight: 1.2 }}>{perfil?.nombreCompleto}</Typography>
        <Typography variant="caption" sx={{ opacity: 0.85 }}>{perfil?.rol}</Typography>
      </Box>
      <Tooltip title="Cuenta">
        <IconButton color="inherit" onClick={abrirMenu} aria-label="Abrir menú de la cuenta">
          <AccountCircle />
        </IconButton>
      </Tooltip>
      <Menu anchorEl={ancla} open={ancla !== null} onClose={() => setAncla(null)}>
        <Box sx={{ px: 2, py: 1 }}>
          <ListItemText primary={perfil?.nombreCompleto} secondary={perfil?.email} />
        </Box>
        <Divider />
        <MenuItem onClick={() => irA(Rutas.MiPerfil)}>Mi perfil</MenuItem>
        <MenuItem onClick={() => irA(Rutas.CambiarContrasena)}>Cambiar contraseña</MenuItem>
        <MenuItem onClick={salir}>Cerrar sesión</MenuItem>
      </Menu>
    </Box>
  )
}
