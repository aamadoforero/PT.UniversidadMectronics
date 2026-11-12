import { createContext, type Context } from 'react'
import type { ValorContextoSesion } from './ValorContextoSesion'

/** Contexto de React que comparte la sesión con toda la aplicación. */
export const ContextoSesion: Context<ValorContextoSesion | null> = createContext<ValorContextoSesion | null>(null)
