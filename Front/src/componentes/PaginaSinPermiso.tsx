import { Alert, Box } from '@mui/material'
import type { ReactNode } from 'react'

/** Mensaje que se muestra cuando el usuario no tiene el permiso de una página. */
export function PaginaSinPermiso(): ReactNode {
  return (
    <Box sx={{ p: 3 }}>
      <Alert severity="warning">No tiene permiso para ver esta página.</Alert>
    </Box>
  )
}
