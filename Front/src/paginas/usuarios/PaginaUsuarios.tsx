import Add from '@mui/icons-material/Add'
import { Alert, Button } from '@mui/material'
import { useSnackbar } from 'notistack'
import { useEffect, useState, type ReactNode } from 'react'
import { obtenerRoles } from '../../api/catalogosApi'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import { buscarUsuarios, cambiarEstadoUsuario, restablecerContrasena } from '../../api/usuariosApi'
import { CargandoPagina } from '../../componentes/CargandoPagina'
import { DialogoConfirmacion } from '../../componentes/DialogoConfirmacion'
import { EncabezadoPagina } from '../../componentes/EncabezadoPagina'
import { CodigosPermiso } from '../../sesion/CodigosPermiso'
import { useSesion } from '../../sesion/useSesion'
import type { ContrasenaRestablecida } from '../../tipos/ContrasenaRestablecida'
import type { FiltroUsuarios } from '../../tipos/FiltroUsuarios'
import type { Pagina } from '../../tipos/Pagina'
import type { Rol } from '../../tipos/Rol'
import type { UsuarioCreado } from '../../tipos/UsuarioCreado'
import type { UsuarioResumen } from '../../tipos/UsuarioResumen'
import type { AccionUsuario } from './AccionUsuario'
import type { ContrasenaGenerada } from './ContrasenaGenerada'
import type { CriteriosFiltro } from './CriteriosFiltro'
import { DialogoContrasenaTemporal } from './DialogoContrasenaTemporal'
import { DialogoUsuario } from './DialogoUsuario'
import { FiltrosUsuarios } from './FiltrosUsuarios'
import { TablaUsuarios } from './TablaUsuarios'

const FILTRO_INICIAL: FiltroUsuarios = { texto: '', rolId: null, activo: null, numeroPagina: 1, tamanoPagina: 20 }

/** Página de administración de cuentas: listado paginado, creación, edición, estado y contraseña temporal. */
export default function PaginaUsuarios(): ReactNode {
  const { tienePermiso } = useSesion()
  const { enqueueSnackbar } = useSnackbar()
  const [filtro, setFiltro] = useState<FiltroUsuarios>(FILTRO_INICIAL)
  const [pagina, setPagina] = useState<Pagina<UsuarioResumen> | null>(null)
  const [cargando, setCargando] = useState<boolean>(true)
  const [errorCarga, setErrorCarga] = useState<string | null>(null)
  const [version, setVersion] = useState<number>(0)
  const [roles, setRoles] = useState<Rol[]>([])
  const [formularioAbierto, setFormularioAbierto] = useState<boolean>(false)
  const [usuarioEditadoId, setUsuarioEditadoId] = useState<number | null>(null)
  const [usuarioConfirmacion, setUsuarioConfirmacion] = useState<UsuarioResumen | null>(null)
  const [accionConfirmacion, setAccionConfirmacion] = useState<AccionUsuario | null>(null)
  const [procesando, setProcesando] = useState<boolean>(false)
  const [contrasenaGenerada, setContrasenaGenerada] = useState<ContrasenaGenerada | null>(null)
  const puedeCrear: boolean = tienePermiso(CodigosPermiso.UsuariosCrear)

  useEffect(() => {
    if (puedeCrear) {
      obtenerRoles().then(setRoles).catch(() => setRoles([]))
    }
  }, [puedeCrear])

  useEffect(() => {
    async function cargarPagina(): Promise<void> {
      setCargando(true)
      try {
        setPagina(await buscarUsuarios(filtro))
        setErrorCarga(null)
      } catch (error: unknown) {
        setErrorCarga(obtenerMensajeError(error))
      } finally {
        setCargando(false)
      }
    }

    cargarPagina()
  }, [filtro, version])

  function recargarListado(): void {
    setVersion(version + 1)
  }

  function aplicarCriterios(criterios: CriteriosFiltro): void {
    setFiltro({ ...filtro, ...criterios, numeroPagina: 1 })
  }

  function abrirFormulario(usuarioId: number | null): void {
    setUsuarioEditadoId(usuarioId)
    setFormularioAbierto(true)
  }

  function cerrarFormulario(): void {
    setFormularioAbierto(false)
    setUsuarioEditadoId(null)
  }

  function mostrarCuentaCreada(creado: UsuarioCreado): void {
    cerrarFormulario()
    enqueueSnackbar('La cuenta se creó.', { variant: 'success' })
    setContrasenaGenerada({ nombreUsuario: creado.usuario.nombreCompleto, contrasena: creado.contrasenaTemporal })
    recargarListado()
  }

  function ejecutarAccion(accion: AccionUsuario, usuario: UsuarioResumen): void {
    if (accion === 'editar') {
      abrirFormulario(usuario.id)
    } else {
      setAccionConfirmacion(accion)
      setUsuarioConfirmacion(usuario)
    }
  }

  function cerrarConfirmacion(): void {
    setUsuarioConfirmacion(null)
    setAccionConfirmacion(null)
  }

  async function confirmarAccion(): Promise<void> {
    if (usuarioConfirmacion === null) {
      return
    }
    setProcesando(true)
    try {
      if (accionConfirmacion === 'cambiarEstado') {
        await cambiarEstadoUsuario(usuarioConfirmacion.id, !usuarioConfirmacion.activo)
        enqueueSnackbar(usuarioConfirmacion.activo ? 'La cuenta se desactivó.' : 'La cuenta se activó.', { variant: 'success' })
      } else {
        const resultado: ContrasenaRestablecida = await restablecerContrasena(usuarioConfirmacion.id)
        setContrasenaGenerada({ nombreUsuario: usuarioConfirmacion.nombreCompleto, contrasena: resultado.contrasenaTemporal })
      }
      cerrarConfirmacion()
      recargarListado()
    } catch (error: unknown) {
      enqueueSnackbar(obtenerMensajeError(error), { variant: 'error' })
    } finally {
      setProcesando(false)
    }
  }

  const desactivando: boolean = usuarioConfirmacion?.activo ?? false
  const esCambioEstado: boolean = accionConfirmacion === 'cambiarEstado'

  return (
    <>
      <EncabezadoPagina
        titulo="Usuarios"
        descripcion="Cuentas de estudiantes, profesores y administradores."
        acciones={puedeCrear && <Button variant="contained" startIcon={<Add />} onClick={() => abrirFormulario(null)}>Nueva cuenta</Button>}
      />
      <FiltrosUsuarios roles={roles} alBuscar={aplicarCriterios} />

      {errorCarga !== null && <Alert severity="error" sx={{ mb: 2 }}>{errorCarga}</Alert>}
      {pagina === null && cargando && <CargandoPagina />}
      {pagina !== null && (
        <TablaUsuarios
          pagina={pagina}
          cargando={cargando}
          alCambiarPagina={(numeroPagina: number) => setFiltro({ ...filtro, numeroPagina })}
          alCambiarTamano={(tamanoPagina: number) => setFiltro({ ...filtro, tamanoPagina, numeroPagina: 1 })}
          alElegirAccion={ejecutarAccion}
        />
      )}

      {formularioAbierto && (
        <DialogoUsuario
          usuarioId={usuarioEditadoId}
          roles={roles}
          alCerrar={cerrarFormulario}
          alCrear={mostrarCuentaCreada}
          alActualizar={() => { cerrarFormulario(); recargarListado() }}
        />
      )}

      <DialogoConfirmacion
        abierto={usuarioConfirmacion !== null}
        titulo={esCambioEstado ? (desactivando ? 'Desactivar cuenta' : 'Activar cuenta') : 'Restablecer contraseña'}
        mensaje={esCambioEstado
          ? (desactivando
            ? `¿Confirma que desea desactivar la cuenta de ${usuarioConfirmacion?.nombreCompleto ?? ''}? Si es un estudiante, se cancelarán sus inscripciones vigentes y no se restaurarán al reactivarla.`
            : `¿Confirma que desea activar la cuenta de ${usuarioConfirmacion?.nombreCompleto ?? ''}?`)
          : `Se generará una contraseña temporal para ${usuarioConfirmacion?.nombreCompleto ?? ''} y la actual dejará de funcionar.`}
        textoConfirmar={esCambioEstado ? (desactivando ? 'Desactivar' : 'Activar') : 'Restablecer'}
        procesando={procesando}
        alConfirmar={confirmarAccion}
        alCancelar={cerrarConfirmacion}
      />

      {contrasenaGenerada !== null && (
        <DialogoContrasenaTemporal contrasenaGenerada={contrasenaGenerada} alCerrar={() => setContrasenaGenerada(null)} />
      )}
    </>
  )
}
