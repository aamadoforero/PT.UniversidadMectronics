using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Seguridad.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IRepositorioInscripciones"/> con Entity Framework.
/// </summary>
/// <param name="contexto">Contexto de la petición en curso.</param>
internal sealed class RepositorioInscripciones(ContextoSeguridad contexto) : IRepositorioInscripciones
{
    /// <summary>
    /// Obtiene las inscripciones vigentes de un estudiante, en seguimiento para poder cancelarlas.
    /// </summary>
    /// <param name="estudianteId">Identificador del perfil de estudiante.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Las inscripciones vigentes.</returns>
    public async Task<IReadOnlyList<Inscripcion>> ObtenerVigentesAsync(int estudianteId, CancellationToken cancelacion) =>
        await contexto.Inscripciones
            .Where(inscripcion => inscripcion.EstudianteId == estudianteId && inscripcion.Activo)
            .ToListAsync(cancelacion);
}
