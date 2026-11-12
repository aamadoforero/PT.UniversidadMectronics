import { CssBaseline, ThemeProvider } from '@mui/material'
import { SnackbarProvider } from 'notistack'
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router'
import { App } from './App'
import { ProveedorSesion } from './sesion/ProveedorSesion'
import { tema } from './tema'

const contenedor: HTMLElement | null = document.getElementById('root')
if (contenedor === null) {
  throw new Error('No se encontró el elemento #root en index.html.')
}

createRoot(contenedor).render(
  <StrictMode>
    <ThemeProvider theme={tema}>
      <CssBaseline />
      <SnackbarProvider maxSnack={3} autoHideDuration={4000} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}>
        <BrowserRouter>
          <ProveedorSesion>
            <App />
          </ProveedorSesion>
        </BrowserRouter>
      </SnackbarProvider>
    </ThemeProvider>
  </StrictMode>,
)
