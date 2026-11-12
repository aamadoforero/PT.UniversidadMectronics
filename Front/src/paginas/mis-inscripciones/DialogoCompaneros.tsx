import { Alert, Avatar, Button, Dialog, DialogActions, DialogContent, DialogTitle, List, ListItem, ListItemAvatar, ListItemText } from '@mui/material'
import { useEffect, useState, type ReactNode } from 'react'
import { obtenerCompaneros } from '../../api/inscripcionesApi'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import { CargandoPagina } from '../../componentes/CargandoPagina'
import type { CompanerosClase } from '../../tipos/CompanerosClase'
import type { Inscripcion } from '../../tipos/Inscripcion'

/** Propiedades del diálogo de compañeros. */
interface PropiedadesDialogoCompaneros {
  inscripcion: Inscripcion
  alCerrar: () => void
}

/**
 * Lista los nombres de los compañeros de una materia inscrita (R9: solo el nombre).
 * @param propiedades Materia inscrita y función para cerrar.
 */
export function DialogoCompaneros({ inscripcion, alCerrar }: PropiedadesDialogoCompaneros): ReactNode {
  const [companeros, setCompaneros] = useState<CompanerosClase | null>(null)
  const [errorCarga, setErrorCarga] = useState<string | null>(null)

  useEffect(() => {
    obtenerCompaneros(inscripcion.materiaId)
      .then(setCompaneros)
      .catch((error: unknown) => setErrorCarga(obtenerMensajeError(error)))
  }, [inscripcion.materiaId])

  return (
    <Dialog open onClose={alCerrar} maxWidth="xs" fullWidth>
      <DialogTitle>Compañeros de {inscripcion.materia}</DialogTitle>
      <DialogContent>
        {errorCarga !== null && <Alert severity="error">{errorCarga}</Alert>}
        {errorCarga === null && companeros === null && <CargandoPagina />}
        {companeros !== null && companeros.companeros.length === 0 && (
          <Alert severity="info">Todavía no hay otros estudiantes inscritos en esta materia.</Alert>
        )}
        {companeros !== null && companeros.companeros.length > 0 && (
          <List dense>
            {companeros.companeros.map((nombre: string, indice: number) => (
              <ListItem key={`${indice}-${nombre}`} disableGutters>
                <ListItemAvatar><Avatar sx={{ bgcolor: 'primary.main', width: 32, height: 32, fontSize: '0.9rem' }}>{nombre.charAt(0)}</Avatar></ListItemAvatar>
                <ListItemText primary={nombre} />
              </ListItem>
            ))}
          </List>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={alCerrar}>Cerrar</Button>
      </DialogActions>
    </Dialog>
  )
}
