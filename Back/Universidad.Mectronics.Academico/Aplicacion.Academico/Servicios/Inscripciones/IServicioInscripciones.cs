using Aplicacion.Academico.Dtos.Inscripciones;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Servicios.Inscripciones;

/// <summary>
/// Inscripción de materias del estudiante autenticado (R1, R5, R7, R9).
/// </summary>
public interface IServicioInscripciones
{
    /// <summary>
    /// Obtiene las materias inscritas por el estudiante y sus créditos.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Las inscripciones vigentes, o un error si la cuenta no tiene perfil de estudiante.</returns>
    Task<Resultado<InscripcionesPropiasDto>> ObtenerPropiasAsync(CancellationToken cancelacion);

    /// <summary>
    /// Inscribe una materia, aplicando las reglas de <c>PoliticaInscripcion</c> dentro de una transacción serializable.
    /// </summary>
    /// <param name="datos">Materia a inscribir.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La inscripción creada, o el error de la regla incumplida.</returns>
    Task<Resultado<InscripcionDto>> InscribirAsync(InscribirMateriaDto datos, CancellationToken cancelacion);

    /// <summary>
    /// Cancela la inscripción de una materia. Queda como historial.
    /// </summary>
    /// <param name="materiaId">Materia a cancelar.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Éxito, o un error si el estudiante no tiene inscrita la materia.</returns>
    Task<Resultado> CancelarAsync(int materiaId, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene los nombres de los compañeros de una clase del estudiante (R9).
    /// </summary>
    /// <param name="materiaId">Materia inscrita.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Los compañeros, o un error si el estudiante no tiene inscrita la materia.</returns>
    Task<Resultado<CompanerosClaseDto>> ObtenerCompanerosAsync(int materiaId, CancellationToken cancelacion);
}
