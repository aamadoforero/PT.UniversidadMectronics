import { Alert, Box, Card, CardContent, Chip, Divider, List, ListItem, ListItemText, Stack, Typography } from '@mui/material'
import { useEffect, useState, type ReactNode } from 'react'
import { obtenerMateriasDictadas } from '../../api/materiasApi'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import { CargandoPagina } from '../../componentes/CargandoPagina'
import { EncabezadoPagina } from '../../componentes/EncabezadoPagina'
import type { MateriaDictada } from '../../tipos/MateriaDictada'

/** Página del profesor con las materias que dicta y los estudiantes inscritos en cada una. */
export default function PaginaMisClases(): ReactNode {
  const [materias, setMaterias] = useState<MateriaDictada[] | null>(null)
  const [errorCarga, setErrorCarga] = useState<string | null>(null)

  useEffect(() => {
    obtenerMateriasDictadas()
      .then(setMaterias)
      .catch((error: unknown) => setErrorCarga(obtenerMensajeError(error)))
  }, [])

  if (errorCarga !== null) {
    return <Alert severity="error">{errorCarga}</Alert>
  }

  if (materias === null) {
    return <CargandoPagina />
  }

  return (
    <>
      <EncabezadoPagina titulo="Mis clases" descripcion="Materias que dicta y estudiantes inscritos." />
      {materias.length === 0 && <Alert severity="info">No tiene materias asignadas.</Alert>}
      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: 'repeat(2, 1fr)' } }}>
        {materias.map((materia: MateriaDictada) => (
          <Card key={materia.materiaId} variant="outlined">
            <CardContent>
              <Stack direction="row" spacing={1} sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
                <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>{materia.materia}</Typography>
                <Chip size="small" color="primary" label={`${materia.creditos} créditos`} />
              </Stack>
              <Typography variant="body2" color="text.secondary">{materia.totalInscritos} estudiantes inscritos</Typography>
              <Divider sx={{ my: 1.5 }} />
              {materia.estudiantes.length === 0
                ? <Typography variant="body2" color="text.secondary">Aún no hay estudiantes inscritos.</Typography>
                : (
                    <List dense disablePadding>
                      {materia.estudiantes.map((nombre: string) => (
                        <ListItem key={nombre} disableGutters><ListItemText primary={nombre} /></ListItem>
                      ))}
                    </List>
                  )}
            </CardContent>
          </Card>
        ))}
      </Box>
    </>
  )
}
