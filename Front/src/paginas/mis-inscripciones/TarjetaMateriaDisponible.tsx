import { Button, Card, CardActions, CardContent, Chip, Stack, Typography } from '@mui/material'
import type { ReactNode } from 'react'
import type { Materia } from '../../tipos/Materia'

/** Propiedades de la tarjeta de materia disponible. */
interface PropiedadesTarjetaMateria {
  materia: Materia
  motivoNoDisponible: string | null
  procesando: boolean
  alInscribir: (materia: Materia) => void
}

/**
 * Materia que el estudiante puede inscribir; si no puede, el botón queda deshabilitado y se muestra el motivo.
 * @param propiedades Materia, motivo de bloqueo y función para inscribir.
 */
export function TarjetaMateriaDisponible({ materia, motivoNoDisponible, procesando, alInscribir }: PropiedadesTarjetaMateria): ReactNode {
  return (
    <Card variant="outlined" sx={{ display: 'flex', flexDirection: 'column', opacity: motivoNoDisponible === null ? 1 : 0.75 }}>
      <CardContent sx={{ flexGrow: 1 }}>
        <Stack direction="row" spacing={1} sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
          <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>{materia.nombre}</Typography>
          <Chip size="small" variant="outlined" label={`${materia.creditos} créditos`} />
        </Stack>
        <Typography variant="body2">Profesor: {materia.profesor}</Typography>
        {motivoNoDisponible !== null && (
          <Typography variant="caption" color="warning.dark" sx={{ display: 'block', mt: 1 }}>{motivoNoDisponible}</Typography>
        )}
      </CardContent>
      <CardActions>
        <Button size="small" variant="contained" disabled={motivoNoDisponible !== null} loading={procesando} onClick={() => alInscribir(materia)}>
          Inscribir
        </Button>
      </CardActions>
    </Card>
  )
}
