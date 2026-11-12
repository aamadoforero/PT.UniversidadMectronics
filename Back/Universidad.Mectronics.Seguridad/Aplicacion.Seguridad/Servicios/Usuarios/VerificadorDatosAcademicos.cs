using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>
/// Verifica que los datos académicos correspondan al tipo de perfil del rol y que el programa y la categoría existan.
/// </summary>
/// <param name="consultaCatalogos">Consulta de programas y categorías.</param>
public sealed class VerificadorDatosAcademicos(IConsultaCatalogos consultaCatalogos)
{
    /// <summary>
    /// Verifica los datos académicos según el tipo de perfil.
    /// </summary>
    /// <param name="tipoPerfil">Tipo de perfil del rol de la cuenta.</param>
    /// <param name="programaId">Programa académico enviado.</param>
    /// <param name="ubicacionSemestral">Semestre enviado.</param>
    /// <param name="categoriaId">Categoría de profesor enviada.</param>
    /// <param name="cancelacion">Permite cancelar las consultas si la petición se abandona.</param>
    /// <returns>Los errores por campo; vacío si los datos son correctos.</returns>
    public async Task<IReadOnlyDictionary<string, string[]>> VerificarAsync(
        TipoPerfil tipoPerfil,
        int? programaId,
        int? ubicacionSemestral,
        byte? categoriaId,
        CancellationToken cancelacion)
    {
        Dictionary<string, string[]> errores = new(StringComparer.Ordinal);
        const string NoAplica = "Este dato no aplica para el rol seleccionado.";

        if (tipoPerfil == TipoPerfil.Estudiante)
        {
            if (programaId is null)
            {
                errores[nameof(CrearUsuarioDto.ProgramaId)] = ["El programa es obligatorio para este rol."];
            }
            else if (!await consultaCatalogos.ExisteProgramaActivoAsync(programaId.Value, cancelacion))
            {
                errores[nameof(CrearUsuarioDto.ProgramaId)] = ["El programa no existe o no está activo."];
            }

            if (ubicacionSemestral is null)
            {
                errores[nameof(CrearUsuarioDto.UbicacionSemestral)] = ["La ubicación semestral es obligatoria para este rol."];
            }
        }
        else if (programaId is not null || ubicacionSemestral is not null)
        {
            if (programaId is not null)
            {
                errores[nameof(CrearUsuarioDto.ProgramaId)] = [NoAplica];
            }

            if (ubicacionSemestral is not null)
            {
                errores[nameof(CrearUsuarioDto.UbicacionSemestral)] = [NoAplica];
            }
        }

        if (tipoPerfil == TipoPerfil.Profesor)
        {
            if (categoriaId is null)
            {
                errores[nameof(CrearUsuarioDto.CategoriaId)] = ["La categoría es obligatoria para este rol."];
            }
            else if (!await consultaCatalogos.ExisteCategoriaProfesorAsync(categoriaId.Value, cancelacion))
            {
                errores[nameof(CrearUsuarioDto.CategoriaId)] = ["La categoría no existe."];
            }
        }
        else if (categoriaId is not null)
        {
            errores[nameof(CrearUsuarioDto.CategoriaId)] = [NoAplica];
        }

        return errores;
    }
}
