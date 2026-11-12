import Search from '@mui/icons-material/Search'
import { Alert, Button, Chip, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TablePagination, TableRow, TextField, Typography } from '@mui/material'
import { useEffect, useState, type ChangeEvent, type FormEvent, type ReactNode } from 'react'
import { buscarEstudiantes } from '../../api/estudiantesApi'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import { CargandoPagina } from '../../componentes/CargandoPagina'
import { EncabezadoPagina } from '../../componentes/EncabezadoPagina'
import type { EstudianteResumen } from '../../tipos/EstudianteResumen'
import type { FiltroEstudiantes } from '../../tipos/FiltroEstudiantes'
import type { Pagina } from '../../tipos/Pagina'

const FILTRO_INICIAL: FiltroEstudiantes = { texto: '', numeroPagina: 1, tamanoPagina: 20 }

/** Página con las inscripciones de todos los estudiantes (R8), paginada y con búsqueda por nombre. */
export default function PaginaTodasInscripciones(): ReactNode {
  const [filtro, setFiltro] = useState<FiltroEstudiantes>(FILTRO_INICIAL)
  const [texto, setTexto] = useState<string>('')
  const [pagina, setPagina] = useState<Pagina<EstudianteResumen> | null>(null)
  const [cargando, setCargando] = useState<boolean>(true)
  const [errorCarga, setErrorCarga] = useState<string | null>(null)

  useEffect(() => {
    async function cargarPagina(): Promise<void> {
      setCargando(true)
      try {
        setPagina(await buscarEstudiantes(filtro))
        setErrorCarga(null)
      } catch (error: unknown) {
        setErrorCarga(obtenerMensajeError(error))
      } finally {
        setCargando(false)
      }
    }

    cargarPagina()
  }, [filtro])

  function enviarBusqueda(evento: FormEvent<HTMLFormElement>): void {
    evento.preventDefault()
    setFiltro({ ...filtro, texto, numeroPagina: 1 })
  }

  return (
    <>
      <EncabezadoPagina titulo="Todas las inscripciones" descripcion="Materias y créditos que tiene inscritos cada estudiante." />

      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack component="form" direction={{ xs: 'column', sm: 'row' }} spacing={2} onSubmit={enviarBusqueda}>
          <TextField label="Buscar por nombre" value={texto} onChange={(evento) => setTexto(evento.target.value)} slotProps={{ htmlInput: { maxLength: 150 } }} />
          <Button type="submit" variant="outlined" startIcon={<Search />} sx={{ flexShrink: 0 }}>Buscar</Button>
        </Stack>
      </Paper>

      {errorCarga !== null && <Alert severity="error" sx={{ mb: 2 }}>{errorCarga}</Alert>}
      {pagina === null && cargando && <CargandoPagina />}
      {pagina !== null && (
        <Paper>
          <TableContainer sx={{ opacity: cargando ? 0.5 : 1, transition: 'opacity 0.2s' }}>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Estudiante</TableCell>
                  <TableCell sx={{ display: { xs: 'none', md: 'table-cell' } }}>Programa</TableCell>
                  <TableCell>Materias</TableCell>
                  <TableCell align="right">Créditos</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {pagina.elementos.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={4}>
                      <Typography color="text.secondary" sx={{ py: 3, textAlign: 'center' }}>No hay estudiantes que coincidan con la búsqueda.</Typography>
                    </TableCell>
                  </TableRow>
                )}
                {pagina.elementos.map((estudiante: EstudianteResumen, indice: number) => (
                  <TableRow key={`${estudiante.nombre}-${indice}`} hover>
                    <TableCell>
                      <Typography variant="body2" sx={{ fontWeight: 600 }}>{estudiante.nombre}</Typography>
                      <Typography variant="caption" color="text.secondary" sx={{ display: { md: 'none' } }}>{estudiante.programa}</Typography>
                    </TableCell>
                    <TableCell sx={{ display: { xs: 'none', md: 'table-cell' } }}>{estudiante.programa}</TableCell>
                    <TableCell>
                      {estudiante.materias.length === 0
                        ? <Typography variant="body2" color="text.secondary">Sin inscripciones</Typography>
                        : (
                            <Stack direction="row" sx={{ flexWrap: 'wrap', gap: 0.5 }}>
                              {estudiante.materias.map((materia: string) => <Chip key={materia} size="small" label={materia} />)}
                            </Stack>
                          )}
                    </TableCell>
                    <TableCell align="right">{estudiante.creditosInscritos}</TableCell>
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
            onPageChange={(_evento: unknown, numero: number) => setFiltro({ ...filtro, numeroPagina: numero + 1 })}
            onRowsPerPageChange={(evento: ChangeEvent<HTMLInputElement>) => setFiltro({ ...filtro, tamanoPagina: Number(evento.target.value), numeroPagina: 1 })}
          />
        </Paper>
      )}
    </>
  )
}
