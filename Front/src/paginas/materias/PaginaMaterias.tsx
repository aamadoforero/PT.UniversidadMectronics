import Add from '@mui/icons-material/Add'
import { Alert, Button, Chip, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material'
import { useSnackbar } from 'notistack'
import { useEffect, useState, type ReactNode } from 'react'
import { obtenerProgramas } from '../../api/catalogosApi'
import { cambiarEstadoMateria, obtenerMaterias } from '../../api/materiasApi'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import { obtenerProfesores } from '../../api/profesoresApi'
import { CargandoPagina } from '../../componentes/CargandoPagina'
import { DialogoConfirmacion } from '../../componentes/DialogoConfirmacion'
import { EncabezadoPagina } from '../../componentes/EncabezadoPagina'
import type { ElementoCatalogo } from '../../tipos/ElementoCatalogo'
import type { Materia } from '../../tipos/Materia'
import type { Profesor } from '../../tipos/Profesor'
import { DialogoMateria } from './DialogoMateria'

const OCULTAR_EN_MOVIL: object = { display: { xs: 'none', md: 'table-cell' } }

/** Página de administración de materias: listado (incluidas las inactivas), creación, edición y cambio de estado. */
export default function PaginaMaterias(): ReactNode {
  const { enqueueSnackbar } = useSnackbar()
  const [materias, setMaterias] = useState<Materia[] | null>(null)
  const [profesores, setProfesores] = useState<Profesor[]>([])
  const [programas, setProgramas] = useState<ElementoCatalogo[]>([])
  const [errorCarga, setErrorCarga] = useState<string | null>(null)
  const [version, setVersion] = useState<number>(0)
  const [formularioAbierto, setFormularioAbierto] = useState<boolean>(false)
  const [materiaEditada, setMateriaEditada] = useState<Materia | null>(null)
  const [materiaEstado, setMateriaEstado] = useState<Materia | null>(null)
  const [procesando, setProcesando] = useState<boolean>(false)

  useEffect(() => {
    obtenerProgramas().then(setProgramas).catch(() => setProgramas([]))
  }, [])

  useEffect(() => {
    Promise.all([obtenerMaterias(true), obtenerProfesores()])
      .then(([listaMaterias, listaProfesores]: [Materia[], Profesor[]]) => {
        setMaterias(listaMaterias)
        setProfesores(listaProfesores)
        setErrorCarga(null)
      })
      .catch((error: unknown) => setErrorCarga(obtenerMensajeError(error)))
  }, [version])

  function recargar(): void {
    setVersion(version + 1)
  }

  function abrirFormulario(materia: Materia | null): void {
    setMateriaEditada(materia)
    setFormularioAbierto(true)
  }

  function cerrarFormulario(): void {
    setFormularioAbierto(false)
    setMateriaEditada(null)
  }

  function obtenerNombrePrograma(programaId: number): string {
    return programas.find((programa: ElementoCatalogo) => programa.id === programaId)?.nombre ?? `Programa ${programaId}`
  }

  async function confirmarCambioEstado(): Promise<void> {
    if (materiaEstado === null) {
      return
    }
    setProcesando(true)
    try {
      await cambiarEstadoMateria(materiaEstado.id, !materiaEstado.activo)
      enqueueSnackbar(materiaEstado.activo ? 'La materia se desactivó.' : 'La materia se activó.', { variant: 'success' })
      setMateriaEstado(null)
      recargar()
    } catch (error: unknown) {
      enqueueSnackbar(obtenerMensajeError(error), { variant: 'error' })
    } finally {
      setProcesando(false)
    }
  }

  if (errorCarga !== null && materias === null) {
    return <Alert severity="error">{errorCarga}</Alert>
  }

  if (materias === null) {
    return <CargandoPagina />
  }

  const desactivando: boolean = materiaEstado?.activo ?? false

  return (
    <>
      <EncabezadoPagina
        titulo="Materias"
        descripcion="Catálogo de materias, créditos y profesor asignado."
        acciones={<Button variant="contained" startIcon={<Add />} onClick={() => abrirFormulario(null)}>Nueva materia</Button>}
      />

      <Paper>
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Materia</TableCell>
                <TableCell sx={OCULTAR_EN_MOVIL}>Programa</TableCell>
                <TableCell>Profesor</TableCell>
                <TableCell align="center" sx={OCULTAR_EN_MOVIL}>Créditos</TableCell>
                <TableCell>Estado</TableCell>
                <TableCell align="right">Acciones</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {materias.length === 0 && (
                <TableRow>
                  <TableCell colSpan={6}>
                    <Typography color="text.secondary" sx={{ py: 3, textAlign: 'center' }}>No hay materias registradas.</Typography>
                  </TableCell>
                </TableRow>
              )}
              {materias.map((materia: Materia) => (
                <TableRow key={materia.id} hover>
                  <TableCell>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>{materia.nombre}</Typography>
                    <Typography variant="caption" color="text.secondary" sx={{ display: { md: 'none' } }}>{materia.creditos} créditos</Typography>
                  </TableCell>
                  <TableCell sx={OCULTAR_EN_MOVIL}>{obtenerNombrePrograma(materia.programaId)}</TableCell>
                  <TableCell>{materia.profesor}</TableCell>
                  <TableCell align="center" sx={OCULTAR_EN_MOVIL}>{materia.creditos}</TableCell>
                  <TableCell>
                    <Chip size="small" label={materia.activo ? 'Activa' : 'Inactiva'} color={materia.activo ? 'success' : 'default'} />
                  </TableCell>
                  <TableCell align="right" sx={{ whiteSpace: 'nowrap' }}>
                    <Button size="small" onClick={() => abrirFormulario(materia)}>Editar</Button>
                    <Button size="small" color={materia.activo ? 'error' : 'success'} onClick={() => setMateriaEstado(materia)}>
                      {materia.activo ? 'Desactivar' : 'Activar'}
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {formularioAbierto && (
        <DialogoMateria
          materia={materiaEditada}
          programas={programas}
          profesores={profesores}
          alCerrar={cerrarFormulario}
          alGuardar={() => { cerrarFormulario(); recargar() }}
        />
      )}

      <DialogoConfirmacion
        abierto={materiaEstado !== null}
        titulo={desactivando ? 'Desactivar materia' : 'Activar materia'}
        mensaje={desactivando
          ? `¿Confirma que desea desactivar ${materiaEstado?.nombre ?? ''}? Los estudiantes no podrán inscribirla.`
          : `¿Confirma que desea activar ${materiaEstado?.nombre ?? ''}?`}
        textoConfirmar={desactivando ? 'Desactivar' : 'Activar'}
        procesando={procesando}
        alConfirmar={confirmarCambioEstado}
        alCancelar={() => setMateriaEstado(null)}
      />
    </>
  )
}
