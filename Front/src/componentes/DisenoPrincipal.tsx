import MenuIcon from '@mui/icons-material/Menu'
import SchoolOutlined from '@mui/icons-material/SchoolOutlined'
import { AppBar, Box, Drawer, IconButton, Toolbar, Typography } from '@mui/material'
import { Suspense, useState, type ReactNode } from 'react'
import { Outlet } from 'react-router'
import { useSesion } from '../sesion/useSesion'
import { CargandoPagina } from './CargandoPagina'
import { MenuCuenta } from './MenuCuenta'
import { MenuNavegacion } from './MenuNavegacion'

const ANCHO_MENU: number = 240

/** Estructura común de las páginas internas: barra superior, menú lateral (desplegable en móviles) y contenido. */
export function DisenoPrincipal(): ReactNode {
  const { perfil } = useSesion()
  const [menuMovilAbierto, setMenuMovilAbierto] = useState<boolean>(false)
  const mostrarMenu: boolean = perfil !== null && !perfil.debeCambiarContrasena

  function cerrarMenuMovil(): void {
    setMenuMovilAbierto(false)
  }

  const contenidoMenu: ReactNode = (
    <>
      <Toolbar />
      <MenuNavegacion alSeleccionar={cerrarMenuMovil} />
    </>
  )

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <AppBar position="fixed" sx={{ zIndex: (tema) => tema.zIndex.drawer + 1 }}>
        <Toolbar>
          {mostrarMenu && (
            <IconButton color="inherit" edge="start" onClick={() => setMenuMovilAbierto(true)} sx={{ mr: 1, display: { md: 'none' } }} aria-label="Abrir menú">
              <MenuIcon />
            </IconButton>
          )}
          <SchoolOutlined sx={{ mr: 1 }} />
          <Typography variant="h6" noWrap sx={{ flexGrow: 1, fontSize: { xs: '1rem', sm: '1.25rem' } }}>
            Universidad Mectronics
          </Typography>
          <MenuCuenta />
        </Toolbar>
      </AppBar>

      {mostrarMenu && (
        <Box component="nav" sx={{ width: { md: ANCHO_MENU }, flexShrink: { md: 0 } }}>
          <Drawer
            variant="temporary"
            open={menuMovilAbierto}
            onClose={cerrarMenuMovil}
            sx={{ display: { xs: 'block', md: 'none' }, '& .MuiDrawer-paper': { width: ANCHO_MENU } }}
          >
            {contenidoMenu}
          </Drawer>
          <Drawer
            variant="permanent"
            open
            sx={{ display: { xs: 'none', md: 'block' }, '& .MuiDrawer-paper': { width: ANCHO_MENU, boxSizing: 'border-box' } }}
          >
            {contenidoMenu}
          </Drawer>
        </Box>
      )}

      <Box component="main" sx={{ flexGrow: 1, minWidth: 0, p: { xs: 2, sm: 3 } }}>
        <Toolbar />
        <Suspense fallback={<CargandoPagina />}>
          <Outlet />
        </Suspense>
      </Box>
    </Box>
  )
}
