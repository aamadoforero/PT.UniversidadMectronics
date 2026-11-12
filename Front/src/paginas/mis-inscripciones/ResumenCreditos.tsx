import { Box, LinearProgress, Paper, Stack, Typography } from '@mui/material'
import type { ReactNode } from 'react'
import type { InscripcionesPropias } from '../../tipos/InscripcionesPropias'

/** Propiedades del resumen de créditos. */
interface PropiedadesResumenCreditos {
  inscripciones: InscripcionesPropias
}

/**
 * Muestra el avance del estudiante en el programa de créditos: créditos y materias frente a sus máximos.
 * @param propiedades Inscripciones del estudiante.
 */
export function ResumenCreditos({ inscripciones }: PropiedadesResumenCreditos): ReactNode {
  const materiasInscritas: number = inscripciones.materias.length

  return (
    <Paper sx={{ p: { xs: 2, sm: 3 }, mb: 3 }}>
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={3}>
        <Box sx={{ flex: 1 }}>
          <Typography variant="body2" color="text.secondary">Créditos inscritos</Typography>
          <Typography variant="h4" component="p" sx={{ fontWeight: 700 }}>
            {inscripciones.creditosInscritos}<Typography component="span" color="text.secondary"> / {inscripciones.creditosMaximos}</Typography>
          </Typography>
          <LinearProgress variant="determinate" value={(inscripciones.creditosInscritos / inscripciones.creditosMaximos) * 100} sx={{ mt: 1, height: 8, borderRadius: 4 }} />
        </Box>
        <Box sx={{ flex: 1 }}>
          <Typography variant="body2" color="text.secondary">Materias inscritas</Typography>
          <Typography variant="h4" component="p" sx={{ fontWeight: 700 }}>
            {materiasInscritas}<Typography component="span" color="text.secondary"> / {inscripciones.materiasMaximas}</Typography>
          </Typography>
          <LinearProgress variant="determinate" color="secondary" value={(materiasInscritas / inscripciones.materiasMaximas) * 100} sx={{ mt: 1, height: 8, borderRadius: 4 }} />
        </Box>
      </Stack>
    </Paper>
  )
}
