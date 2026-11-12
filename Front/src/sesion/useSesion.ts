import { useContext } from 'react'
import { ContextoSesion } from './ContextoSesion'
import type { ValorContextoSesion } from './ValorContextoSesion'

/**
 * Obtiene la sesión actual. React exige que los hooks empiecen con `use`.
 * @returns Perfil, estado de carga y acciones de sesión.
 */
export function useSesion(): ValorContextoSesion {
  const valor: ValorContextoSesion | null = useContext(ContextoSesion)
  if (valor === null) {
    throw new Error('useSesion debe usarse dentro de ProveedorSesion.')
  }
  return valor
}
