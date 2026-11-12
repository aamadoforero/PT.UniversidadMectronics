using Aplicacion.Academico.Dtos.Estudiantes;
using Aplicacion.Academico.Interfaces;
using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;
using Universidad.Mectronics.Transversal.Paginacion;

namespace Infraestructura.Academico.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IRepositorioEstudiantes"/> con Entity Framework. Solo carga las inscripciones vigentes
/// (inclusión filtrada), que son las que usan las reglas y las consultas.
/// </summary>
/// <param name="contexto">Contexto de la petición.</param>
internal sealed class RepositorioEstudiantes(ContextoAcademico contexto) : IRepositorioEstudiantes
{
    /// <summary>Carácter de escape de los comodines de <c>LIKE</c>.</summary>
    private const string CaracterEscape = "\\";

    /// <summary>
    /// Obtiene el estudiante de una cuenta con sus inscripciones vigentes, en seguimiento.
    /// </summary>
    /// <param name="usuarioId">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El estudiante, o <see langword="null"/> si no existe.</returns>
    public Task<Estudiante?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancelacion) =>
        IncluirDetalle(contexto.Estudiantes).FirstOrDefaultAsync(estudiante => estudiante.UsuarioId == usuarioId, cancelacion);

    /// <summary>
    /// Busca estudiantes con cuenta activa, ordenados por nombre y paginados.
    /// </summary>
    /// <param name="filtro">Texto a buscar en el nombre y página solicitada.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La página de estudiantes.</returns>
    public async Task<Pagina<Estudiante>> BuscarAsync(BuscarEstudiantesDto filtro, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        IQueryable<Estudiante> consulta = contexto.Estudiantes
            .AsNoTracking()
            .Where(estudiante => estudiante.Usuario.Activo);

        if (filtro.Texto is not null)
        {
            string patron = $"%{EscaparPatronLike(filtro.Texto)}%";
            consulta = consulta.Where(estudiante => EF.Functions.Like(estudiante.Usuario.NombreCompleto, patron, CaracterEscape));
        }

        int total = await consulta.CountAsync(cancelacion);
        List<Estudiante> elementos = await IncluirDetalle(consulta)
            .OrderBy(estudiante => estudiante.Usuario.NombreCompleto)
            .ThenBy(estudiante => estudiante.Id)
            .Skip((filtro.NumeroPagina - 1) * filtro.TamanoPagina)
            .Take(filtro.TamanoPagina)
            .AsSplitQuery()
            .ToListAsync(cancelacion);

        return new Pagina<Estudiante>(elementos, filtro.NumeroPagina, filtro.TamanoPagina, total);
    }

    /// <summary>
    /// Obtiene los nombres de los estudiantes activos inscritos en una materia.
    /// </summary>
    /// <param name="materiaId">Materia.</param>
    /// <param name="estudianteExcluidoId">Estudiante que se omite.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Los nombres, ordenados alfabéticamente.</returns>
    public async Task<IReadOnlyList<string>> ObtenerNombresInscritosAsync(int materiaId, int estudianteExcluidoId, CancellationToken cancelacion) =>
        await contexto.Estudiantes
            .AsNoTracking()
            .Where(estudiante => estudiante.Id != estudianteExcluidoId &&
                                 estudiante.Usuario.Activo &&
                                 estudiante.Inscripciones.Any(inscripcion => inscripcion.Activo && inscripcion.MateriaId == materiaId))
            .OrderBy(estudiante => estudiante.Usuario.NombreCompleto)
            .Select(estudiante => estudiante.Usuario.NombreCompleto)
            .ToListAsync(cancelacion);

    /// <summary>
    /// Agrega a la consulta la cuenta, el programa y las inscripciones vigentes con su materia y el profesor de cada una.
    /// </summary>
    /// <param name="consulta">Consulta de estudiantes.</param>
    /// <returns>La consulta con las inclusiones.</returns>
    private static IQueryable<Estudiante> IncluirDetalle(IQueryable<Estudiante> consulta) => consulta
        .Include(estudiante => estudiante.Usuario)
        .Include(estudiante => estudiante.Programa)
        .Include(estudiante => estudiante.Inscripciones.Where(inscripcion => inscripcion.Activo))
            .ThenInclude(inscripcion => inscripcion.Materia)
                .ThenInclude(materia => materia.Profesor)
                    .ThenInclude(profesor => profesor.Usuario);

    /// <summary>
    /// Escapa los comodines de <c>LIKE</c> para que el texto se busque literal.
    /// </summary>
    /// <param name="texto">Texto recibido.</param>
    /// <returns>El texto con <c>%</c>, <c>_</c>, <c>[</c> y el escape precedidos por el carácter de escape.</returns>
    private static string EscaparPatronLike(string texto) => texto
        .Replace(CaracterEscape, CaracterEscape + CaracterEscape, StringComparison.Ordinal)
        .Replace("%", CaracterEscape + "%", StringComparison.Ordinal)
        .Replace("_", CaracterEscape + "_", StringComparison.Ordinal)
        .Replace("[", CaracterEscape + "[", StringComparison.Ordinal);
}
