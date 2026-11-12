using Dominio.Academico.Entidades;

namespace Aplicacion.Academico.Interfaces;

/// <summary>
/// Acceso de solo lectura a los profesores.
/// </summary>
public interface IRepositorioProfesores
{
    /// <summary>
    /// Obtiene los profesores con cuenta activa, con su nombre, ordenados alfabéticamente.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Los profesores.</returns>
    Task<IReadOnlyList<Profesor>> ObtenerTodosAsync(CancellationToken cancelacion);

    /// <summary>
    /// Obtiene un profesor con cuenta activa, con su nombre.
    /// </summary>
    /// <param name="id">Identificador del perfil de profesor.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El profesor, o <see langword="null"/> si no existe o su cuenta está inactiva.</returns>
    Task<Profesor?> ObtenerActivoPorIdAsync(int id, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene el profesor de una cuenta.
    /// </summary>
    /// <param name="usuarioId">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>El profesor, o <see langword="null"/> si la cuenta no tiene perfil de profesor.</returns>
    Task<Profesor?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancelacion);
}
