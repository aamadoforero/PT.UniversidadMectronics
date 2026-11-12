using Aplicacion.Academico.Dtos.Estudiantes;
using Aplicacion.Academico.Interfaces;
using Dominio.Academico.Entidades;
using Universidad.Mectronics.Transversal.Paginacion;

namespace PruebasUnitarias.Academico.Simuladores;

/// <summary>
/// Repositorio de estudiantes en memoria.
/// </summary>
/// <param name="estudiantes">Estudiantes disponibles.</param>
public sealed class RepositorioEstudiantesEnMemoria(params Estudiante[] estudiantes) : IRepositorioEstudiantes
{
    /// <summary>Estudiantes disponibles.</summary>
    public IReadOnlyList<Estudiante> Estudiantes { get; } = estudiantes;

    /// <summary>
    /// Busca por cuenta.
    /// </summary>
    /// <param name="usuarioId">Identificador de la cuenta.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>El estudiante, o <see langword="null"/>.</returns>
    public Task<Estudiante?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancelacion) =>
        Task.FromResult(Estudiantes.FirstOrDefault(estudiante => estudiante.UsuarioId == usuarioId));

    /// <summary>
    /// Filtra por nombre y pagina.
    /// </summary>
    /// <param name="filtro">Criterios.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>La página.</returns>
    public Task<Pagina<Estudiante>> BuscarAsync(BuscarEstudiantesDto filtro, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(filtro);
        List<Estudiante> filtrados = Estudiantes
            .Where(estudiante => filtro.Texto is null || estudiante.Usuario.NombreCompleto.Contains(filtro.Texto, StringComparison.OrdinalIgnoreCase))
            .OrderBy(estudiante => estudiante.Usuario.NombreCompleto, StringComparer.Ordinal)
            .ToList();
        List<Estudiante> pagina = filtrados.Skip((filtro.NumeroPagina - 1) * filtro.TamanoPagina).Take(filtro.TamanoPagina).ToList();
        return Task.FromResult(new Pagina<Estudiante>(pagina, filtro.NumeroPagina, filtro.TamanoPagina, filtrados.Count));
    }

    /// <summary>
    /// Obtiene los nombres de los inscritos en la materia.
    /// </summary>
    /// <param name="materiaId">Materia.</param>
    /// <param name="estudianteExcluidoId">Estudiante que se omite.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Los nombres, ordenados.</returns>
    public Task<IReadOnlyList<string>> ObtenerNombresInscritosAsync(int materiaId, int estudianteExcluidoId, CancellationToken cancelacion)
    {
        IReadOnlyList<string> nombres = Estudiantes
            .Where(estudiante => estudiante.Id != estudianteExcluidoId && estudiante.TieneInscrita(materiaId))
            .Select(estudiante => estudiante.Usuario.NombreCompleto)
            .Order(StringComparer.Ordinal)
            .ToList();
        return Task.FromResult(nombres);
    }
}
