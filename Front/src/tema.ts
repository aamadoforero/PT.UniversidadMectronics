import { createTheme, type Theme } from '@mui/material/styles'

/** Tema visual de la aplicación (colores institucionales y bordes). */
export const tema: Theme = createTheme({
  palette: {
    primary: { main: '#1e4b8f' },
    secondary: { main: '#c8902b' },
    background: { default: '#f4f6fa' },
  },
  shape: { borderRadius: 10 },
  typography: {
    fontFamily: '"Roboto", "Segoe UI", system-ui, sans-serif',
    button: { textTransform: 'none', fontWeight: 600 },
  },
  components: {
    MuiTextField: { defaultProps: { fullWidth: true, size: 'small' } },
    MuiPaper: { defaultProps: { elevation: 0 }, styleOverrides: { root: { border: '1px solid #e2e6ee' } } },
    MuiAppBar: { styleOverrides: { root: { border: 'none' } } },
  },
})
