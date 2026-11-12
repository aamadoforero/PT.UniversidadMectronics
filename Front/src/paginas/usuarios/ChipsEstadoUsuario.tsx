import { Chip, Stack } from '@mui/material'
import type { ReactNode } from 'react'
import type { UsuarioResumen } from '../../tipos/UsuarioResumen'

/** Propiedades de los indicadores de estado. */
interface PropiedadesChipsEstado {
  usuario: UsuarioResumen
}

/**
 * Indicadores de estado de una cuenta: activa o inactiva, bloqueada y con contraseña temporal.
 * @param propiedades Cuenta que se muestra.
 */
export function ChipsEstadoUsuario({ usuario }: PropiedadesChipsEstado): ReactNode {
  return (
    <Stack direction="row" spacing={0.5} sx={{ flexWrap: 'wrap', gap: 0.5 }}>
      <Chip size="small" label={usuario.activo ? 'Activa' : 'Inactiva'} color={usuario.activo ? 'success' : 'default'} />
      {usuario.estaBloqueado && <Chip size="small" label="Bloqueada" color="error" variant="outlined" />}
      {usuario.debeCambiarContrasena && <Chip size="small" label="Contraseña temporal" color="warning" variant="outlined" />}
    </Stack>
  )
}
