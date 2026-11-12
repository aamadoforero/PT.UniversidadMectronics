using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Acceso a las inscripciones vigentes de un estudiante, para cancelarlas al desactivar su cuenta.
/// </summary>
public interface IRepositorioInscripciones
{
    /// <summary>
    /// Obtiene las inscripciones vigentes de un estudiante, en seguimiento para poder cancelarlas.
    /// </summary>
    /// <param name="estudianteId">Identificador del perfil de estudiante.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>Las inscripciones vigentes.</returns>
    Task<IReadOnlyList<Inscripcion>> ObtenerVigentesAsync(int estudianteId, CancellationToken cancelacion);
}
