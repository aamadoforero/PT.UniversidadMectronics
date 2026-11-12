import { Paper, Table, TableBody, TableCell, TableContainer, TableHead, TablePagination, TableRow, Typography } from '@mui/material'
import type { ChangeEvent, ReactNode } from 'react'
import type { Pagina } from '../../tipos/Pagina'
import type { UsuarioResumen } from '../../tipos/UsuarioResumen'
import type { AccionUsuario } from './AccionUsuario'
import { ChipsEstadoUsuario } from './ChipsEstadoUsuario'
import { MenuAccionesUsuario } from './MenuAccionesUsuario'

const OCULTAR_EN_MOVIL: object = { display: { xs: 'none', md: 'table-cell' } }

/** Propiedades de la tabla de usuarios. */
interface PropiedadesTablaUsuarios {
  pagina: Pagina<UsuarioResumen>
  cargando: boolean
  alCambiarPagina: (numeroPagina: number) => void
  alCambiarTamano: (tamanoPagina: number) => void
  alElegirAccion: (accion: AccionUsuario, usuario: UsuarioResumen) => void
}

/**
 * Tabla paginada de cuentas. En pantallas pequeñas oculta identificación y correo.
 * @param propiedades Página de datos y funciones de paginación y acciones.
 */
export function TablaUsuarios(propiedades: PropiedadesTablaUsuarios): ReactNode {
  const { pagina, cargando } = propiedades

  return (
    <Paper>
      <TableContainer sx={{ opacity: cargando ? 0.5 : 1, transition: 'opacity 0.2s' }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Nombre</TableCell>
              <TableCell sx={OCULTAR_EN_MOVIL}>Identificación</TableCell>
              <TableCell sx={OCULTAR_EN_MOVIL}>Correo</TableCell>
              <TableCell>Rol</TableCell>
              <TableCell>Estado</TableCell>
              <TableCell align="right" />
            </TableRow>
          </TableHead>
          <TableBody>
            {pagina.elementos.length === 0 && (
              <TableRow>
                <TableCell colSpan={6}>
                  <Typography color="text.secondary" sx={{ py: 3, textAlign: 'center' }}>No hay cuentas que coincidan con la búsqueda.</Typography>
                </TableCell>
              </TableRow>
            )}
            {pagina.elementos.map((usuario: UsuarioResumen) => (
              <TableRow key={usuario.id} hover>
                <TableCell>
                  <Typography variant="body2" sx={{ fontWeight: 600 }}>{usuario.nombreCompleto}</Typography>
                  <Typography variant="caption" color="text.secondary" sx={{ display: { md: 'none' } }}>{usuario.email}</Typography>
                </TableCell>
                <TableCell sx={OCULTAR_EN_MOVIL}>{usuario.identificacion}</TableCell>
                <TableCell sx={OCULTAR_EN_MOVIL}>{usuario.email}</TableCell>
                <TableCell>{usuario.rol}</TableCell>
                <TableCell><ChipsEstadoUsuario usuario={usuario} /></TableCell>
                <TableCell align="right">
                  <MenuAccionesUsuario usuario={usuario} alElegir={propiedades.alElegirAccion} />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={pagina.totalElementos}
        page={pagina.numeroPagina - 1}
        rowsPerPage={pagina.tamanoPagina}
        rowsPerPageOptions={[10, 20, 50]}
        labelRowsPerPage="Filas"
        labelDisplayedRows={({ from, to, count }: { from: number; to: number; count: number }): string => `${from}–${to} de ${count}`}
        onPageChange={(_evento: unknown, numero: number) => propiedades.alCambiarPagina(numero + 1)}
        onRowsPerPageChange={(evento: ChangeEvent<HTMLInputElement>) => propiedades.alCambiarTamano(Number(evento.target.value))}
      />
    </Paper>
  )
}
