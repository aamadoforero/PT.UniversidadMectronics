import { Alert, Button, Dialog, DialogActions, DialogContent, DialogTitle, MenuItem, Stack, TextField, useMediaQuery, useTheme } from '@mui/material'
import { useSnackbar } from 'notistack'
import type { ReactNode } from 'react'
import { Controller, useForm, type UseFormReturn } from 'react-hook-form'
import { actualizarMateria, crearMateria } from '../../api/materiasApi'
import { obtenerMensajeError } from '../../api/obtenerMensajeError'
import type { DatosMateria } from '../../tipos/DatosMateria'
import type { ElementoCatalogo } from '../../tipos/ElementoCatalogo'
import type { Materia } from '../../tipos/Materia'
import type { Profesor } from '../../tipos/Profesor'
import { aplicarErroresServidor } from '../../validaciones/aplicarErroresServidor'
import type { FormularioMateria } from './FormularioMateria'

const LONGITUD_MAXIMA_NOMBRE: number = 100

/** Propiedades del diálogo de materia. */
interface PropiedadesDialogoMateria {
  materia: Materia | null
  programas: ElementoCatalogo[]
  profesores: Profesor[]
  alCerrar: () => void
  alGuardar: () => void
}

/**
 * Formulario para crear o editar una materia. El profesor se elige entre los que tienen cupo (máximo 2 materias, R6).
 * @param propiedades Materia a editar (`null` para crear), catálogos y funciones de cierre y guardado.
 */
export function DialogoMateria({ materia, programas, profesores, alCerrar, alGuardar }: PropiedadesDialogoMateria): ReactNode {
  const esCreacion: boolean = materia === null
  const pantallaPequena: boolean = useMediaQuery(useTheme().breakpoints.down('sm'))
  const { enqueueSnackbar } = useSnackbar()
  const formulario: UseFormReturn<FormularioMateria> = useForm<FormularioMateria>({
    defaultValues: {
      nombre: materia?.nombre ?? '',
      programaId: materia === null ? '' : String(materia.programaId),
      profesorId: materia === null ? '' : String(materia.profesorId),
    },
  })
  const { register, control, handleSubmit, setError, formState } = formulario

  function tieneCupo(profesor: Profesor): boolean {
    return profesor.materiasAsignadas < profesor.materiasMaximas || profesor.id === materia?.profesorId
  }

  async function enviarFormulario(valores: FormularioMateria): Promise<void> {
    const datos: DatosMateria = {
      nombre: valores.nombre.trim(),
      profesorId: Number(valores.profesorId),
      programaId: esCreacion ? Number(valores.programaId) : undefined,
    }
    try {
      if (materia === null) {
        await crearMateria(datos)
        enqueueSnackbar('La materia se creó.', { variant: 'success' })
      } else {
        await actualizarMateria(materia.id, datos)
        enqueueSnackbar('La materia se actualizó.', { variant: 'success' })
      }
      alGuardar()
    } catch (error: unknown) {
      aplicarErroresServidor(error, setError)
      enqueueSnackbar(obtenerMensajeError(error), { variant: 'error' })
    }
  }

  return (
    <Dialog open onClose={alCerrar} maxWidth="sm" fullWidth fullScreen={pantallaPequena}>
      <DialogTitle>{esCreacion ? 'Nueva materia' : 'Editar materia'}</DialogTitle>
      <DialogContent>
        <Stack component="form" id="formulario-materia" spacing={2} sx={{ pt: 1 }} onSubmit={handleSubmit(enviarFormulario)} noValidate>
          <TextField
            label="Nombre"
            {...register('nombre', {
              validate: (valor: string): true | string => {
                const texto: string = valor.trim()
                if (texto === '') {
                  return 'El nombre es obligatorio.'
                }
                return texto.length <= LONGITUD_MAXIMA_NOMBRE || `El nombre admite máximo ${LONGITUD_MAXIMA_NOMBRE} caracteres.`
              },
            })}
            error={formState.errors.nombre !== undefined}
            helperText={formState.errors.nombre?.message}
          />
          <TextField label="Créditos" value="3" disabled helperText="Cada materia equivale a 3 créditos." />
          {esCreacion
            ? (
                <Controller
                  name="programaId"
                  control={control}
                  rules={{ required: 'El programa es obligatorio.' }}
                  render={({ field, fieldState }) => (
                    <TextField select label="Programa" {...field} error={fieldState.error !== undefined} helperText={fieldState.error?.message}>
                      {programas.map((programa: ElementoCatalogo) => <MenuItem key={programa.id} value={String(programa.id)}>{programa.nombre}</MenuItem>)}
                    </TextField>
                  )}
                />
              )
            : (
                <TextField
                  label="Programa"
                  value={programas.find((programa: ElementoCatalogo) => programa.id === materia?.programaId)?.nombre ?? ''}
                  disabled
                  helperText="El programa no se cambia después de crear la materia."
                />
              )}
          <Controller
            name="profesorId"
            control={control}
            rules={{ required: 'El profesor es obligatorio.' }}
            render={({ field, fieldState }) => (
              <TextField select label="Profesor" {...field} error={fieldState.error !== undefined} helperText={fieldState.error?.message ?? 'Cada profesor dicta máximo 2 materias.'}>
                {profesores.map((profesor: Profesor) => (
                  <MenuItem key={profesor.id} value={String(profesor.id)} disabled={!tieneCupo(profesor)}>
                    {profesor.nombre} ({profesor.materiasAsignadas}/{profesor.materiasMaximas})
                  </MenuItem>
                ))}
              </TextField>
            )}
          />
          {!esCreacion && (
            <Alert severity="info">Si la materia tiene estudiantes inscritos, no se puede cambiar el profesor.</Alert>
          )}
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={alCerrar} disabled={formState.isSubmitting}>Cancelar</Button>
        <Button type="submit" form="formulario-materia" variant="contained" loading={formState.isSubmitting}>
          {esCreacion ? 'Crear materia' : 'Guardar cambios'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
