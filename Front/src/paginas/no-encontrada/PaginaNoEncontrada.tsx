import { Box, Button, Typography } from '@mui/material'
import type { ReactNode } from 'react'
import { Link } from 'react-router'
import { Rutas } from '../../rutas'

/** Página para rutas que no existen. */
export default function PaginaNoEncontrada(): ReactNode {
  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: 2, p: 2, textAlign: 'center' }}>
      <Typography variant="h3" component="h1" sx={{ fontWeight: 700 }}>404</Typography>
      <Typography color="text.secondary">La página que busca no existe.</Typography>
      <Button component={Link} to={Rutas.Inicio} variant="contained">Volver al inicio</Button>
    </Box>
  )
}
