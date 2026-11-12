using Aplicacion.Academico.Interfaces;
using Dominio.Academico.Entidades;

namespace PruebasUnitarias.Academico.Simuladores;

/// <summary>
/// Repositorio de profesores en memoria.
/// </summary>
/// <param name="profesores">Profesores disponibles.</param>
public sealed class RepositorioProfesoresEnMemoria(IReadOnlyList<Profesor> profesores) : IRepositorioProfesores
{
    /// <summary>
    /// Obtiene los profesores con cuenta activa.
    /// </summary>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Los profesores.</returns>
    public Task<IReadOnlyList<Profesor>> ObtenerTodosAsync(CancellationToken cancelacion)
    {
        IReadOnlyList<Profesor> activos = profesores.Where(profesor => profesor.Usuario.Activo).ToList();
        return Task.FromResult(activos);
    }

    /// <summary>
    /// Busca un profesor con cuenta activa.
    /// </summary>
    /// <param name="id">Identificador.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>El profesor, o <see langword="null"/>.</returns>
    public Task<Profesor?> ObtenerActivoPorIdAsync(int id, CancellationToken cancelacion) =>
        Task.FromResult(profesores.FirstOrDefault(profesor => profesor.Id == id && profesor.Usuario.Activo));

    /// <summary>
    /// Busca el profesor de una cuenta.
    /// </summary>
    /// <param name="usuarioId">Identificador de la cuenta.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>El profesor, o <see langword="null"/>.</returns>
    public Task<Profesor?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancelacion) =>
        Task.FromResult(profesores.FirstOrDefault(profesor => profesor.UsuarioId == usuarioId));
}
