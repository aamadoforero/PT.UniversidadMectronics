import CheckCircle from '@mui/icons-material/CheckCircle'
import RadioButtonUnchecked from '@mui/icons-material/RadioButtonUnchecked'
import { List, ListItem, ListItemIcon, ListItemText } from '@mui/material'
import type { ReactNode } from 'react'
import type { CondicionContrasena } from '../../validaciones/CondicionContrasena'
import { condicionesContrasena } from '../../validaciones/politicaContrasena'

/** Propiedades de la lista de condiciones. */
interface PropiedadesListaCondiciones {
  contrasena: string
}

/**
 * Muestra en vivo qué condiciones de la política cumple la contraseña escrita.
 * @param propiedades Contraseña que se está escribiendo.
 */
export function ListaCondicionesContrasena({ contrasena }: PropiedadesListaCondiciones): ReactNode {
  return (
    <List dense disablePadding>
      {condicionesContrasena.map((condicion: CondicionContrasena) => {
        const cumple: boolean = contrasena !== '' && condicion.cumple(contrasena)
        return (
          <ListItem key={condicion.descripcion} disableGutters sx={{ py: 0 }}>
            <ListItemIcon sx={{ minWidth: 28 }}>
              {cumple ? <CheckCircle color="success" fontSize="small" /> : <RadioButtonUnchecked color="disabled" fontSize="small" />}
            </ListItemIcon>
            <ListItemText primary={condicion.descripcion} slotProps={{ primary: { variant: 'body2', color: cumple ? 'text.primary' : 'text.secondary' } }} />
          </ListItem>
        )
      })}
    </List>
  )
}
