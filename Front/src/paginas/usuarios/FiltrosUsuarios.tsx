import Search from '@mui/icons-material/Search'
import { Button, MenuItem, Paper, Stack, TextField } from '@mui/material'
import { useState, type FormEvent, type ReactNode } from 'react'
import type { Rol } from '../../tipos/Rol'
import type { CriteriosFiltro } from './CriteriosFiltro'

/** Propiedades de los filtros. */
interface PropiedadesFiltrosUsuarios {
  roles: Rol[]
  alBuscar: (criterios: CriteriosFiltro) => void
}

/**
 * Filtros del listado: texto (nombre, correo o identificación), rol y estado.
 * @param propiedades Roles disponibles y función que recibe los criterios.
 */
export function FiltrosUsuarios({ roles, alBuscar }: PropiedadesFiltrosUsuarios): ReactNode {
  const [texto, setTexto] = useState<string>('')
  const [rolId, setRolId] = useState<string>('')
  const [estado, setEstado] = useState<string>('')

  function enviarFiltros(evento: FormEvent<HTMLFormElement>): void {
    evento.preventDefault()
    alBuscar({
      texto,
      rolId: rolId === '' ? null : Number(rolId),
      activo: estado === '' ? null : estado === 'activa',
    })
  }

  return (
    <Paper sx={{ p: 2, mb: 2 }}>
      <Stack component="form" direction={{ xs: 'column', md: 'row' }} spacing={2} onSubmit={enviarFiltros}>
        <TextField label="Buscar" placeholder="Nombre, correo o identificación" value={texto} onChange={(evento) => setTexto(evento.target.value)} slotProps={{ htmlInput: { maxLength: 150 } }} />
        {roles.length > 0 && (
          <TextField select label="Rol" value={rolId} onChange={(evento) => setRolId(evento.target.value)} sx={{ minWidth: { md: 200 } }}>
            <MenuItem value="">Todos</MenuItem>
            {roles.map((rol: Rol) => <MenuItem key={rol.id} value={String(rol.id)}>{rol.nombre}</MenuItem>)}
          </TextField>
        )}
        <TextField select label="Estado" value={estado} onChange={(evento) => setEstado(evento.target.value)} sx={{ minWidth: { md: 160 } }}>
          <MenuItem value="">Todos</MenuItem>
          <MenuItem value="activa">Activas</MenuItem>
          <MenuItem value="inactiva">Inactivas</MenuItem>
        </TextField>
        <Button type="submit" variant="outlined" startIcon={<Search />} sx={{ flexShrink: 0 }}>Buscar</Button>
      </Stack>
    </Paper>
  )
}
