import { Alert, Box, Typography } from '@mui/material'
import { useSnackbar } from 'notistack'
import { useEffect, useState, type ReactNode } from 'react'
import { cancelarInscripcion, inscribirMateria, obtenerInscripcionesPropias } from '../../api/inscripcionesApi'
import { obtenerMaterias } from '../../api/materiasApi'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import { CargandoPagina } from '../../componentes/CargandoPagina'
import { DialogoConfirmacion } from '../../componentes/DialogoConfirmacion'
import { EncabezadoPagina } from '../../componentes/EncabezadoPagina'
import { useSesion } from '../../sesion/useSesion'
import type { Inscripcion } from '../../tipos/Inscripcion'
import type { InscripcionesPropias } from '../../tipos/InscripcionesPropias'
import type { Materia } from '../../tipos/Materia'
import { DialogoCompaneros } from './DialogoCompaneros'
import { obtenerMotivoNoDisponible } from './obtenerMotivoNoDisponible'
import { ResumenCreditos } from './ResumenCreditos'
import { TarjetaInscripcion } from './TarjetaInscripcion'
import { TarjetaMateriaDisponible } from './TarjetaMateriaDisponible'

const CUADRICULA: object = { display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)', lg: 'repeat(3, 1fr)' } }

/** Página del estudiante para inscribir y cancelar materias de su programa y ver a sus compañeros. */
export default function PaginaMisInscripciones(): ReactNode {
  const { perfil } = useSesion()
  const { enqueueSnackbar } = useSnackbar()
  const [inscripciones, setInscripciones] = useState<InscripcionesPropias | null>(null)
  const [catalogo, setCatalogo] = useState<Materia[]>([])
  const [errorCarga, setErrorCarga] = useState<string | null>(null)
  const [version, setVersion] = useState<number>(0)
  const [materiaEnProceso, setMateriaEnProceso] = useState<number | null>(null)
  const [inscripcionCompaneros, setInscripcionCompaneros] = useState<Inscripcion | null>(null)
  const [inscripcionCancelacion, setInscripcionCancelacion] = useState<Inscripcion | null>(null)
  const [cancelando, setCancelando] = useState<boolean>(false)
  const programaId: number | null = perfil?.perfilEstudiante?.programaId ?? null

  useEffect(() => {
    Promise.all([obtenerInscripcionesPropias(), obtenerMaterias(false)])
      .then(([propias, materias]: [InscripcionesPropias, Materia[]]) => {
        setInscripciones(propias)
        setCatalogo(materias)
        setErrorCarga(null)
      })
      .catch((error: unknown) => setErrorCarga(obtenerMensajeError(error)))
  }, [version])

  function recargar(): void {
    setVersion(version + 1)
  }

  async function inscribir(materia: Materia): Promise<void> {
    setMateriaEnProceso(materia.id)
    try {
      await inscribirMateria(materia.id)
      enqueueSnackbar(`Se inscribió ${materia.nombre}.`, { variant: 'success' })
      recargar()
    } catch (error: unknown) {
      enqueueSnackbar(obtenerMensajeError(error), { variant: 'error' })
    } finally {
      setMateriaEnProceso(null)
    }
  }

  async function confirmarCancelacion(): Promise<void> {
    if (inscripcionCancelacion === null) {
      return
    }
    setCancelando(true)
    try {
      await cancelarInscripcion(inscripcionCancelacion.materiaId)
      enqueueSnackbar(`Se canceló la inscripción de ${inscripcionCancelacion.materia}.`, { variant: 'success' })
      setInscripcionCancelacion(null)
      recargar()
    } catch (error: unknown) {
      enqueueSnackbar(obtenerMensajeError(error), { variant: 'error' })
    } finally {
      setCancelando(false)
    }
  }

  if (errorCarga !== null && inscripciones === null) {
    return <Alert severity="error">{errorCarga}</Alert>
  }

  if (inscripciones === null) {
    return <CargandoPagina />
  }

  const idsInscritos: Set<number> = new Set(inscripciones.materias.map((inscripcion: Inscripcion) => inscripcion.materiaId))
  const disponibles: Materia[] = catalogo.filter(
    (materia: Materia) => materia.programaId === programaId && !idsInscritos.has(materia.id),
  )

  return (
    <>
      <EncabezadoPagina
        titulo="Mis inscripciones"
        descripcion={`Cada estudiante puede inscribir hasta ${inscripciones.materiasMaximas} materias y no puede repetir profesor.`}
      />
      <ResumenCreditos inscripciones={inscripciones} />

      <Typography variant="h6" component="h2" sx={{ mb: 2 }}>Materias inscritas</Typography>
      {inscripciones.materias.length === 0
        ? <Alert severity="info" sx={{ mb: 3 }}>Aún no tiene materias inscritas. Elija entre las disponibles.</Alert>
        : (
            <Box sx={{ ...CUADRICULA, mb: 4 }}>
              {inscripciones.materias.map((inscripcion: Inscripcion) => (
                <TarjetaInscripcion
                  key={inscripcion.materiaId}
                  inscripcion={inscripcion}
                  alVerCompaneros={setInscripcionCompaneros}
                  alCancelar={setInscripcionCancelacion}
                />
              ))}
            </Box>
          )}

      <Typography variant="h6" component="h2" sx={{ mb: 2 }}>Materias disponibles de su programa</Typography>
      {disponibles.length === 0
        ? <Alert severity="info">No hay más materias disponibles en su programa.</Alert>
        : (
            <Box sx={CUADRICULA}>
              {disponibles.map((materia: Materia) => (
                <TarjetaMateriaDisponible
                  key={materia.id}
                  materia={materia}
                  motivoNoDisponible={obtenerMotivoNoDisponible(materia, inscripciones)}
                  procesando={materiaEnProceso === materia.id}
                  alInscribir={inscribir}
                />
              ))}
            </Box>
          )}

      {inscripcionCompaneros !== null && (
        <DialogoCompaneros inscripcion={inscripcionCompaneros} alCerrar={() => setInscripcionCompaneros(null)} />
      )}

      <DialogoConfirmacion
        abierto={inscripcionCancelacion !== null}
        titulo="Cancelar inscripción"
        mensaje={`¿Confirma que desea cancelar la inscripción de ${inscripcionCancelacion?.materia ?? ''}?`}
        textoConfirmar="Cancelar inscripción"
        textoCancelar="Volver"
        procesando={cancelando}
        alConfirmar={confirmarCancelacion}
        alCancelar={() => setInscripcionCancelacion(null)}
      />
    </>
  )
}
