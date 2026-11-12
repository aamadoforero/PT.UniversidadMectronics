import { Alert, Button, Dialog, DialogActions, DialogContent, DialogTitle, MenuItem, Stack, TextField, useMediaQuery, useTheme } from '@mui/material'
import { useSnackbar } from 'notistack'
import { useEffect, useState, type ReactNode } from 'react'
import { Controller, useForm, useWatch, type UseFormReturn } from 'react-hook-form'
import { obtenerCategoriasProfesor, obtenerProgramas } from '../../api/catalogosApi'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import { actualizarUsuario, crearUsuario, obtenerUsuario } from '../../api/usuariosApi'
import { CargandoPagina } from '../../componentes/CargandoPagina'
import type { DatosUsuario } from '../../tipos/DatosUsuario'
import type { ElementoCatalogo } from '../../tipos/ElementoCatalogo'
import type { Rol } from '../../tipos/Rol'
import type { TipoPerfil } from '../../tipos/TipoPerfil'
import type { UsuarioCreado } from '../../tipos/UsuarioCreado'
import type { UsuarioDetalle } from '../../tipos/UsuarioDetalle'
import { aplicarErroresServidor } from '../../validaciones/aplicarErroresServidor'
import { validarCorreo, validarIdentificacion, validarNombreCompleto } from '../../validaciones/reglasCampos'
import { convertirFormularioUsuario } from './convertirFormularioUsuario'
import type { FormularioUsuario } from './FormularioUsuario'

const VALORES_INICIALES: FormularioUsuario = {
  nombreCompleto: '', identificacion: '', email: '', rolId: '', programaId: '', ubicacionSemestral: '', categoriaId: '',
}

/** Propiedades del diálogo de cuenta. */
interface PropiedadesDialogoUsuario {
  usuarioId: number | null
  roles: Rol[]
  alCerrar: () => void
  alCrear: (creado: UsuarioCreado) => void
  alActualizar: () => void
}

/**
 * Formulario para crear una cuenta (con rol) o editar sus datos. Los datos académicos dependen del tipo de perfil del rol.
 * @param propiedades `usuarioId` nulo para crear; roles y funciones de cierre y guardado.
 */
export function DialogoUsuario({ usuarioId, roles, alCerrar, alCrear, alActualizar }: PropiedadesDialogoUsuario): ReactNode {
  const esCreacion: boolean = usuarioId === null
  const pantallaPequena: boolean = useMediaQuery(useTheme().breakpoints.down('sm'))
  const { enqueueSnackbar } = useSnackbar()
  const [detalle, setDetalle] = useState<UsuarioDetalle | null>(null)
  const [programas, setProgramas] = useState<ElementoCatalogo[]>([])
  const [categorias, setCategorias] = useState<ElementoCatalogo[]>([])
  const [cargando, setCargando] = useState<boolean>(true)
  const [errorCarga, setErrorCarga] = useState<string | null>(null)
  const formulario: UseFormReturn<FormularioUsuario> = useForm<FormularioUsuario>({ defaultValues: VALORES_INICIALES, shouldUnregister: true })
  const { register, control, handleSubmit, setError, reset, formState } = formulario
  const rolIdElegido: string = useWatch({ control, name: 'rolId' })
  const rolElegido: Rol | undefined = roles.find((rol: Rol) => String(rol.id) === rolIdElegido)
  const tipoPerfil: TipoPerfil = (esCreacion ? rolElegido?.tipoPerfil : detalle?.tipoPerfil) ?? 'Ninguno'

  useEffect(() => {
    async function cargarDatos(): Promise<void> {
      try {
        const [listaProgramas, listaCategorias]: [ElementoCatalogo[], ElementoCatalogo[]] = await Promise.all([obtenerProgramas(), obtenerCategoriasProfesor()])
        setProgramas(listaProgramas)
        setCategorias(listaCategorias)
        if (usuarioId !== null) {
          const usuario: UsuarioDetalle = await obtenerUsuario(usuarioId)
          setDetalle(usuario)
          reset(convertirDetalleAFormulario(usuario))
        }
      } catch (error: unknown) {
        setErrorCarga(obtenerMensajeError(error))
      } finally {
        setCargando(false)
      }
    }

    cargarDatos()
  }, [usuarioId, reset])

  async function enviarFormulario(valores: FormularioUsuario): Promise<void> {
    const datos: DatosUsuario = convertirFormularioUsuario({ ...VALORES_INICIALES, ...valores }, tipoPerfil, esCreacion)
    try {
      if (usuarioId === null) {
        alCrear(await crearUsuario(datos))
      } else {
        await actualizarUsuario(usuarioId, datos)
        enqueueSnackbar('La cuenta se actualizó.', { variant: 'success' })
        alActualizar()
      }
    } catch (error: unknown) {
      aplicarErroresServidor(error, setError)
      enqueueSnackbar(obtenerMensajeError(error), { variant: 'error' })
    }
  }

  return (
    <Dialog open onClose={alCerrar} maxWidth="sm" fullWidth fullScreen={pantallaPequena}>
      <DialogTitle>{esCreacion ? 'Nueva cuenta' : 'Editar cuenta'}</DialogTitle>
      <DialogContent>
        {cargando && <CargandoPagina />}
        {errorCarga !== null && <Alert severity="error">{errorCarga}</Alert>}
        {!cargando && errorCarga === null && (
          <Stack component="form" id="formulario-usuario" spacing={2} sx={{ pt: 1 }} onSubmit={handleSubmit(enviarFormulario)} noValidate>
            <TextField label="Nombre completo" {...register('nombreCompleto', { validate: validarNombreCompleto })} error={formState.errors.nombreCompleto !== undefined} helperText={formState.errors.nombreCompleto?.message} />
            <TextField label="Identificación" {...register('identificacion', { validate: validarIdentificacion })} error={formState.errors.identificacion !== undefined} helperText={formState.errors.identificacion?.message} />
            <TextField label="Correo" type="email" {...register('email', { validate: validarCorreo })} error={formState.errors.email !== undefined} helperText={formState.errors.email?.message} />

            {esCreacion
              ? (
                  <Controller
                    name="rolId"
                    control={control}
                    rules={{ required: 'El rol es obligatorio.' }}
                    render={({ field, fieldState }) => (
                      <TextField select label="Rol" {...field} error={fieldState.error !== undefined} helperText={fieldState.error?.message}>
                        {roles.map((rol: Rol) => <MenuItem key={rol.id} value={String(rol.id)}>{rol.nombre}</MenuItem>)}
                      </TextField>
                    )}
                  />
                )
              : <TextField label="Rol" value={detalle?.rol ?? ''} disabled helperText="El rol no se puede cambiar después de crear la cuenta." />}

            {tipoPerfil === 'Estudiante' && (
              <>
                <Controller
                  name="programaId"
                  control={control}
                  rules={{ required: 'El programa es obligatorio para este rol.' }}
                  render={({ field, fieldState }) => (
                    <TextField select label="Programa" {...field} error={fieldState.error !== undefined} helperText={fieldState.error?.message}>
                      {programas.map((programa: ElementoCatalogo) => <MenuItem key={programa.id} value={String(programa.id)}>{programa.nombre}</MenuItem>)}
                    </TextField>
                  )}
                />
                <TextField
                  label="Ubicación semestral"
                  type="number"
                  slotProps={{ htmlInput: { min: 1, max: 10 } }}
                  {...register('ubicacionSemestral', {
                    validate: (valor: string): true | string => {
                      const semestre: number = Number(valor)
                      return (valor !== '' && Number.isInteger(semestre) && semestre >= 1 && semestre <= 10)
                        || 'La ubicación semestral debe estar entre 1 y 10.'
                    },
                  })}
                  error={formState.errors.ubicacionSemestral !== undefined}
                  helperText={formState.errors.ubicacionSemestral?.message}
                />
              </>
            )}

            {tipoPerfil === 'Profesor' && (
              <Controller
                name="categoriaId"
                control={control}
                rules={{ required: 'La categoría es obligatoria para este rol.' }}
                render={({ field, fieldState }) => (
                  <TextField select label="Categoría de vinculación" {...field} error={fieldState.error !== undefined} helperText={fieldState.error?.message}>
                    {categorias.map((categoria: ElementoCatalogo) => <MenuItem key={categoria.id} value={String(categoria.id)}>{categoria.nombre}</MenuItem>)}
                  </TextField>
                )}
              />
            )}

            {esCreacion && (
              <Alert severity="info">El sistema generará una contraseña temporal que se mostrará al guardar.</Alert>
            )}
          </Stack>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={alCerrar} disabled={formState.isSubmitting}>Cancelar</Button>
        <Button type="submit" form="formulario-usuario" variant="contained" loading={formState.isSubmitting} disabled={cargando || errorCarga !== null}>
          {esCreacion ? 'Crear cuenta' : 'Guardar cambios'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

function convertirDetalleAFormulario(usuario: UsuarioDetalle): FormularioUsuario {
  return {
    nombreCompleto: usuario.nombreCompleto,
    identificacion: usuario.identificacion,
    email: usuario.email,
    rolId: String(usuario.rolId),
    programaId: usuario.perfilEstudiante === null ? '' : String(usuario.perfilEstudiante.programaId),
    ubicacionSemestral: usuario.perfilEstudiante === null ? '' : String(usuario.perfilEstudiante.ubicacionSemestral),
    categoriaId: usuario.perfilProfesor === null ? '' : String(usuario.perfilProfesor.categoriaId),
  }
}
