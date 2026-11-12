import ContentCopy from '@mui/icons-material/ContentCopy'
import { Alert, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, IconButton, Tooltip, Typography } from '@mui/material'
import { useSnackbar } from 'notistack'
import type { ReactNode } from 'react'
import type { ContrasenaGenerada } from './ContrasenaGenerada'

/** Propiedades del diálogo de contraseña temporal. */
interface PropiedadesDialogoContrasena {
  contrasenaGenerada: ContrasenaGenerada
  alCerrar: () => void
}

/**
 * Muestra una sola vez la contraseña temporal generada por el backend, con opción de copiarla.
 * @param propiedades Contraseña generada y función para cerrar.
 */
export function DialogoContrasenaTemporal({ contrasenaGenerada, alCerrar }: PropiedadesDialogoContrasena): ReactNode {
  const { enqueueSnackbar } = useSnackbar()

  async function copiarContrasena(): Promise<void> {
    try {
      await navigator.clipboard.writeText(contrasenaGenerada.contrasena)
      enqueueSnackbar('Contraseña copiada.', { variant: 'info' })
    } catch {
      enqueueSnackbar('No se pudo copiar; selecciónela y cópiela manualmente.', { variant: 'warning' })
    }
  }

  return (
    <Dialog open onClose={alCerrar} maxWidth="xs" fullWidth>
      <DialogTitle>Contraseña temporal</DialogTitle>
      <DialogContent>
        <Typography sx={{ mb: 2 }}>
          Entregue esta contraseña a <strong>{contrasenaGenerada.nombreUsuario}</strong>. Deberá cambiarla al iniciar sesión.
        </Typography>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, p: 1.5, bgcolor: 'grey.100', borderRadius: 2 }}>
          <Typography sx={{ fontFamily: 'monospace', fontSize: '1.1rem', flexGrow: 1, wordBreak: 'break-all' }}>
            {contrasenaGenerada.contrasena}
          </Typography>
          <Tooltip title="Copiar">
            <IconButton onClick={copiarContrasena} aria-label="Copiar contraseña"><ContentCopy /></IconButton>
          </Tooltip>
        </Box>
        <Alert severity="warning" sx={{ mt: 2 }}>No se volverá a mostrar.</Alert>
      </DialogContent>
      <DialogActions>
        <Button variant="contained" onClick={alCerrar}>Entendido</Button>
      </DialogActions>
    </Dialog>
  )
}
