import { Alert, Button, Paper, Stack, TextField } from '@mui/material'
import { useSnackbar } from 'notistack'
import type { ReactNode } from 'react'
import { useForm, useWatch, type UseFormReturn } from 'react-hook-form'
import { useNavigate, type NavigateFunction } from 'react-router'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import * as sesionApi from '../../api/sesionApi'
import { EncabezadoPagina } from '../../componentes/EncabezadoPagina'
import { Rutas } from '../../rutas'
import { useSesion } from '../../sesion/useSesion'
import { aplicarErroresServidor } from '../../validaciones/aplicarErroresServidor'
import { validarContrasena } from '../../validaciones/politicaContrasena'
import type { FormularioCambiarContrasena } from './FormularioCambiarContrasena'
import { ListaCondicionesContrasena } from './ListaCondicionesContrasena'

/** Página para cambiar la contraseña; es obligatoria cuando la cuenta tiene una contraseña temporal. */
export default function PaginaCambiarContrasena(): ReactNode {
  const { perfil, recargarPerfil } = useSesion()
  const { enqueueSnackbar } = useSnackbar()
  const navegar: NavigateFunction = useNavigate()
  const formulario: UseFormReturn<FormularioCambiarContrasena> = useForm<FormularioCambiarContrasena>({
    defaultValues: { contrasenaActual: '', contrasenaNueva: '', confirmacion: '' },
  })
  const { register, control, handleSubmit, setError, formState } = formulario
  const contrasenaNueva: string = useWatch({ control, name: 'contrasenaNueva' })
  const esObligatorio: boolean = perfil?.debeCambiarContrasena ?? false

  async function enviarFormulario(valores: FormularioCambiarContrasena): Promise<void> {
    try {
      await sesionApi.cambiarContrasena(valores.contrasenaActual, valores.contrasenaNueva)
      await recargarPerfil()
      enqueueSnackbar('La contraseña se cambió correctamente.', { variant: 'success' })
      navegar(Rutas.Inicio, { replace: true })
    } catch (error: unknown) {
      aplicarErroresServidor(error, setError)
      enqueueSnackbar(obtenerMensajeError(error), { variant: 'error' })
    }
  }

  return (
    <Stack sx={{ maxWidth: 520 }}>
      <EncabezadoPagina titulo="Cambiar contraseña" />
      {esObligatorio && (
        <Alert severity="info" sx={{ mb: 2 }}>
          Su cuenta tiene una contraseña temporal. Debe cambiarla para continuar.
        </Alert>
      )}
      <Paper sx={{ p: { xs: 2, sm: 3 } }}>
        <Stack component="form" spacing={2} onSubmit={handleSubmit(enviarFormulario)} noValidate>
          <TextField
            label={esObligatorio ? 'Contraseña temporal' : 'Contraseña actual'}
            type="password"
            autoComplete="current-password"
            {...register('contrasenaActual', { required: 'La contraseña actual es obligatoria.' })}
            error={formState.errors.contrasenaActual !== undefined}
            helperText={formState.errors.contrasenaActual?.message}
          />
          <TextField
            label="Contraseña nueva"
            type="password"
            autoComplete="new-password"
            {...register('contrasenaNueva', {
              validate: {
                politica: validarContrasena,
                distinta: (valor: string, valores: FormularioCambiarContrasena): true | string =>
                  valor !== valores.contrasenaActual || 'La contraseña nueva debe ser distinta de la actual.',
              },
            })}
            error={formState.errors.contrasenaNueva !== undefined}
            helperText={formState.errors.contrasenaNueva?.message}
          />
          <ListaCondicionesContrasena contrasena={contrasenaNueva} />
          <TextField
            label="Confirmar contraseña nueva"
            type="password"
            autoComplete="new-password"
            {...register('confirmacion', {
              validate: (valor: string, valores: FormularioCambiarContrasena): true | string =>
                valor === valores.contrasenaNueva || 'Las contraseñas no coinciden.',
            })}
            error={formState.errors.confirmacion !== undefined}
            helperText={formState.errors.confirmacion?.message}
          />
          <Button type="submit" variant="contained" loading={formState.isSubmitting} sx={{ alignSelf: 'flex-start' }}>
            Guardar contraseña
          </Button>
        </Stack>
      </Paper>
    </Stack>
  )
}
