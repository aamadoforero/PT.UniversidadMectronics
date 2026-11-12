/** Materia que dicta el profesor, con los nombres de sus estudiantes. */
export interface MateriaDictada {
  materiaId: number
  materia: string
  creditos: number
  totalInscritos: number
  estudiantes: string[]
}
