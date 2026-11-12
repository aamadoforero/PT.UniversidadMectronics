import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material'
import type { ReactNode } from 'react'

/** Propiedades del diálogo de confirmación. */
interface PropiedadesDialogoConfirmacion {
  abierto: boolean
  titulo: string
  mensaje: string
  textoConfirmar: string
  textoCancelar?: string
  procesando: boolean
  alConfirmar: () => void
  alCancelar: () => void
}

/**
 * Pide confirmación antes de una acción sensible (desactivar una cuenta, restablecer su contraseña).
 * @param propiedades Textos y acciones del diálogo.
 */
export function DialogoConfirmacion(propiedades: PropiedadesDialogoConfirmacion): ReactNode {
  return (
    <Dialog open={propiedades.abierto} onClose={propiedades.alCancelar} maxWidth="xs" fullWidth>
      <DialogTitle>{propiedades.titulo}</DialogTitle>
      <DialogContent>
        <DialogContentText>{propiedades.mensaje}</DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={propiedades.alCancelar} disabled={propiedades.procesando}>{propiedades.textoCancelar ?? 'Cancelar'}</Button>
        <Button variant="contained" onClick={propiedades.alConfirmar} loading={propiedades.procesando}>
          {propiedades.textoConfirmar}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
