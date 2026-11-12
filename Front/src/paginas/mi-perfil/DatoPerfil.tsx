import { Box, Typography } from '@mui/material'
import type { ReactNode } from 'react'

/** Propiedades de un dato del perfil. */
interface PropiedadesDatoPerfil {
  etiqueta: string
  valor: string
}

/**
 * Muestra una etiqueta con su valor, en modo solo lectura.
 * @param propiedades Etiqueta y valor.
 */
export function DatoPerfil({ etiqueta, valor }: PropiedadesDatoPerfil): ReactNode {
  return (
    <Box>
      <Typography variant="caption" color="text.secondary">{etiqueta}</Typography>
      <Typography sx={{ wordBreak: 'break-word' }}>{valor}</Typography>
    </Box>
  )
}
