import { Button, Divider, Paper, Stack, Typography } from '@mui/material'
import { type ReactNode } from 'react'
import { Link } from 'react-router'
import { EncabezadoPagina } from '../../componentes/EncabezadoPagina'
import { Rutas } from '../../rutas'
import { useSesion } from '../../sesion/useSesion'
import type { PerfilEstudiante } from '../../tipos/PerfilEstudiante'
import type { PerfilProfesor } from '../../tipos/PerfilProfesor'
import { DatoPerfil } from './DatoPerfil'

/**
 * Describe el dato académico de la cuenta con los nombres que entrega el perfil.
 * @param estudiante Perfil de estudiante, si la cuenta lo tiene.
 * @param profesor Perfil de profesor, si la cuenta lo tiene.
 * @returns El programa y el semestre, la categoría, o null si la cuenta no tiene perfil académico.
 */
function describirDatoAcademico(estudiante: PerfilEstudiante | null, profesor: PerfilProfesor | null): string | null {
  if (estudiante !== null) {
    return `${estudiante.nombrePrograma ?? `Programa ${estudiante.programaId}`} · Semestre ${estudiante.ubicacionSemestral}`
  }

  if (profesor !== null) {
    return profesor.nombreCategoria ?? `Categoría ${profesor.categoriaId}`
  }

  return null
}

/** Página de solo lectura con los datos de la cuenta del usuario. */
export default function PaginaMiPerfil(): ReactNode {
  const { perfil } = useSesion()

  if (perfil === null) {
    return null
  }

  const datoAcademico: string | null = describirDatoAcademico(perfil.perfilEstudiante, perfil.perfilProfesor)

  return (
    <Stack sx={{ maxWidth: 640 }}>
      <EncabezadoPagina titulo="Mi perfil" descripcion="Datos de su cuenta." />
      <Paper sx={{ p: { xs: 2, sm: 3 } }}>
        <Stack spacing={2}>
          <DatoPerfil etiqueta="Nombre completo" valor={perfil.nombreCompleto} />
          <DatoPerfil etiqueta="Correo" valor={perfil.email} />
          <DatoPerfil etiqueta="Identificación" valor={perfil.identificacion} />
          <DatoPerfil etiqueta="Rol" valor={perfil.rol} />
          {datoAcademico !== null && (
            <DatoPerfil etiqueta={perfil.perfilEstudiante ? 'Programa' : 'Categoría de vinculación'} valor={datoAcademico} />
          )}
          <Divider />
          <Typography variant="body2" color="text.secondary">
            Si algún dato no es correcto, solicite el cambio a un administrador.
          </Typography>
          <Button component={Link} to={Rutas.CambiarContrasena} variant="outlined" sx={{ alignSelf: 'flex-start' }}>
            Cambiar contraseña
          </Button>
        </Stack>
      </Paper>
    </Stack>
  )
}
