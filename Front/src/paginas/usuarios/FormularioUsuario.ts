/** Valores del formulario de cuenta. Los campos de selección se manejan como texto ('' = sin elegir). */
export interface FormularioUsuario {
  nombreCompleto: string
  identificacion: string
  email: string
  rolId: string
  programaId: string
  ubicacionSemestral: string
  categoriaId: string
}
