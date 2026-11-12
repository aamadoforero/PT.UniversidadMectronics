import { Box, CircularProgress } from '@mui/material'
import type { ReactNode } from 'react'

/** Indicador de carga centrado, mientras llega una página o sus datos. */
export function CargandoPagina(): ReactNode {
  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
      <CircularProgress aria-label="Cargando" />
    </Box>
  )
}
