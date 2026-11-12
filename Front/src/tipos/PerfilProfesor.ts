/** Datos académicos de una cuenta de profesor. */
export interface PerfilProfesor {
  categoriaId: number
  /** Nombre de la categoría; lo entrega Seguridad para no consultar el catálogo. */
  nombreCategoria: string | null
}
