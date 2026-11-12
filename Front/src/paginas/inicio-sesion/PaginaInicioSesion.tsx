import SchoolOutlined from '@mui/icons-material/SchoolOutlined'
import Visibility from '@mui/icons-material/Visibility'
import VisibilityOff from '@mui/icons-material/VisibilityOff'
import { Alert, Box, Button, IconButton, InputAdornment, Paper, Stack, TextField, Typography } from '@mui/material'
import { useState, type ReactNode } from 'react'
import { useForm, type UseFormReturn } from 'react-hook-form'
import { Navigate, useLocation, useNavigate, type Location, type NavigateFunction } from 'react-router'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import { Rutas } from '../../rutas'
import { useSesion } from '../../sesion/useSesion'
import { aplicarErroresServidor } from '../../validaciones/aplicarErroresServidor'
import { validarCorreo } from '../../validaciones/reglasCampos'
import type { FormularioInicioSesion } from './FormularioInicioSesion'

/** Página pública de inicio de sesión. */
export default function PaginaInicioSesion(): ReactNode {
  const { perfil, iniciarSesion } = useSesion()
  const navegar: NavigateFunction = useNavigate()
  const ubicacion: Location = useLocation()
  const [mostrarContrasena, setMostrarContrasena] = useState<boolean>(false)
  const [mensajeError, setMensajeError] = useState<string | null>(null)
  const formulario: UseFormReturn<FormularioInicioSesion> = useForm<FormularioInicioSesion>({
    defaultValues: { email: '', contrasena: '' },
  })
  const { register, handleSubmit, setError, formState } = formulario
  const destino: string = (ubicacion.state as { desde?: string } | null)?.desde ?? Rutas.Inicio

  if (perfil !== null) {
    return <Navigate to={destino} replace />
  }

  async function enviarFormulario(valores: FormularioInicioSesion): Promise<void> {
    setMensajeError(null)
    try {
      await iniciarSesion(valores.email.trim(), valores.contrasena)
      navegar(destino, { replace: true })
    } catch (error: unknown) {
      aplicarErroresServidor(error, setError)
      setMensajeError(obtenerMensajeError(error))
    }
  }

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', p: 2, background: 'linear-gradient(135deg, #1e4b8f 0%, #0f2a52 100%)' }}>
      <Paper sx={{ width: '100%', maxWidth: 400, p: { xs: 3, sm: 4 } }}>
        <Stack spacing={1} sx={{ alignItems: 'center', mb: 3 }}>
          <SchoolOutlined color="primary" sx={{ fontSize: 48 }} />
          <Typography variant="h5" component="h1" sx={{ fontWeight: 700 }}>Universidad Mectronics</Typography>
          <Typography color="text.secondary">Ingrese con su cuenta institucional</Typography>
        </Stack>

        <Stack component="form" spacing={2} onSubmit={handleSubmit(enviarFormulario)} noValidate>
          {mensajeError !== null && <Alert severity="error">{mensajeError}</Alert>}
          <TextField
            label="Correo"
            type="email"
            autoComplete="username"
            autoFocus
            {...register('email', { validate: validarCorreo })}
            error={formState.errors.email !== undefined}
            helperText={formState.errors.email?.message}
          />
          <TextField
            label="Contraseña"
            type={mostrarContrasena ? 'text' : 'password'}
            autoComplete="current-password"
            {...register('contrasena', { required: 'La contraseña es obligatoria.' })}
            error={formState.errors.contrasena !== undefined}
            helperText={formState.errors.contrasena?.message}
            slotProps={{
              input: {
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => setMostrarContrasena(!mostrarContrasena)} edge="end" aria-label="Mostrar u ocultar la contraseña">
                      {mostrarContrasena ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              },
            }}
          />
          <Button type="submit" variant="contained" size="large" loading={formState.isSubmitting}>
            Iniciar sesión
          </Button>
        </Stack>
      </Paper>
    </Box>
  )
}
