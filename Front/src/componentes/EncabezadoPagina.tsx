import { Box, Typography } from '@mui/material'
import type { ReactNode } from 'react'

/** Propiedades del encabezado de página. */
interface PropiedadesEncabezadoPagina {
  titulo: string
  descripcion?: string
  acciones?: ReactNode
}

/**
 * Título, descripción y botones principales de una página.
 * @param propiedades Textos y acciones del encabezado.
 */
export function EncabezadoPagina({ titulo, descripcion, acciones }: PropiedadesEncabezadoPagina): ReactNode {
  return (
    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
      <Box>
        <Typography variant="h5" component="h1" sx={{ fontWeight: 700 }}>{titulo}</Typography>
        {descripcion !== undefined && <Typography color="text.secondary">{descripcion}</Typography>}
      </Box>
      {acciones}
    </Box>
  )
}
