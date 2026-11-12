import { Chip, Paper, Stack, Typography } from '@mui/material'
import type { ReactNode } from 'react'
import { EncabezadoPagina } from '../../componentes/EncabezadoPagina'
import { useSesion } from '../../sesion/useSesion'

/** Página de bienvenida después de iniciar sesión. */
export default function PaginaInicio(): ReactNode {
  const { perfil } = useSesion()

  return (
    <>
      <EncabezadoPagina titulo={`Hola, ${perfil?.nombreCompleto ?? ''}`} descripcion="Bienvenido al sistema de registro académico." />
      <Paper sx={{ p: { xs: 2, sm: 3 } }}>
        <Stack spacing={1.5}>
          <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>Su cuenta</Typography>
          <Typography>Correo: {perfil?.email}</Typography>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <Typography>Rol:</Typography>
            <Chip label={perfil?.rol} color="primary" size="small" />
          </Stack>
          <Typography color="text.secondary" variant="body2">
            Use el menú para acceder a las opciones disponibles según sus permisos.
          </Typography>
        </Stack>
      </Paper>
    </>
  )
}
