import { Button, Card, CardActions, CardContent, Chip, Stack, Typography } from '@mui/material'
import type { ReactNode } from 'react'
import type { Inscripcion } from '../../tipos/Inscripcion'

/** Propiedades de la tarjeta de inscripción. */
interface PropiedadesTarjetaInscripcion {
  inscripcion: Inscripcion
  alVerCompaneros: (inscripcion: Inscripcion) => void
  alCancelar: (inscripcion: Inscripcion) => void
}

/**
 * Materia inscrita, con acciones para ver los compañeros y cancelar la inscripción.
 * @param propiedades Inscripción y funciones de las acciones.
 */
export function TarjetaInscripcion({ inscripcion, alVerCompaneros, alCancelar }: PropiedadesTarjetaInscripcion): ReactNode {
  const fecha: string = new Date(inscripcion.fechaInscripcion).toLocaleDateString('es-CO', { dateStyle: 'medium' })

  return (
    <Card variant="outlined" sx={{ display: 'flex', flexDirection: 'column', borderColor: 'primary.light' }}>
      <CardContent sx={{ flexGrow: 1 }}>
        <Stack direction="row" spacing={1} sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
          <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>{inscripcion.materia}</Typography>
          <Chip size="small" color="primary" label={`${inscripcion.creditos} créditos`} />
        </Stack>
        <Typography variant="body2">Profesor: {inscripcion.profesor}</Typography>
        <Typography variant="caption" color="text.secondary">Inscrita el {fecha}</Typography>
      </CardContent>
      <CardActions sx={{ justifyContent: 'space-between' }}>
        <Button size="small" onClick={() => alVerCompaneros(inscripcion)}>Ver compañeros</Button>
        <Button size="small" color="error" onClick={() => alCancelar(inscripcion)}>Cancelar inscripción</Button>
      </CardActions>
    </Card>
  )
}
