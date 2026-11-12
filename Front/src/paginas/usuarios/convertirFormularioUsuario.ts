import type { DatosUsuario } from '../../tipos/DatosUsuario'
import type { TipoPerfil } from '../../tipos/TipoPerfil'
import type { FormularioUsuario } from './FormularioUsuario'

/**
 * Convierte los valores del formulario al cuerpo que espera el backend.
 * @param valores Valores escritos en el formulario.
 * @param tipoPerfil Tipo de perfil del rol, que decide qué datos académicos se envían.
 * @param incluirRol `true` al crear (el rol solo se envía en la creación).
 * @returns Datos listos para enviar.
 */
export function convertirFormularioUsuario(valores: FormularioUsuario, tipoPerfil: TipoPerfil, incluirRol: boolean): DatosUsuario {
  const esEstudiante: boolean = tipoPerfil === 'Estudiante'
  const esProfesor: boolean = tipoPerfil === 'Profesor'

  return {
    nombreCompleto: valores.nombreCompleto.trim(),
    identificacion: valores.identificacion.trim(),
    email: valores.email.trim(),
    rolId: incluirRol ? Number(valores.rolId) : undefined,
    programaId: esEstudiante ? Number(valores.programaId) : null,
    ubicacionSemestral: esEstudiante ? Number(valores.ubicacionSemestral) : null,
    categoriaId: esProfesor ? Number(valores.categoriaId) : null,
  }
}
