using Dominio.Academico.Politicas;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Errores;

/// <summary>
/// Errores esperados de las inscripciones.
/// </summary>
public static class ErroresInscripcion
{
    /// <summary>
    /// Crea el error para datos de entrada con formato inválido.
    /// </summary>
    /// <param name="detalles">Mensajes de error por campo.</param>
    /// <returns>El error de validación.</returns>
    public static Error CrearDatosInvalidos(IReadOnlyDictionary<string, string[]> detalles) => Error.CrearValidacion(
        "Inscripcion.DatosInvalidos",
        "La inscripción tiene datos inválidos.",
        detalles);

    /// <summary>
    /// Crea el error para una cuenta sin perfil de estudiante.
    /// </summary>
    /// <returns>El error de falta de permiso.</returns>
    public static Error CrearSinPerfilEstudiante() => Error.CrearProhibido(
        "Inscripcion.SinPerfilEstudiante",
        "Su cuenta no tiene perfil de estudiante.");

    /// <summary>
    /// Crea el error para una materia que no existe.
    /// </summary>
    /// <param name="materiaId">Materia buscada.</param>
    /// <returns>El error de recurso no encontrado.</returns>
    public static Error CrearMateriaNoEncontrada(int materiaId) => Error.CrearNoEncontrado(
        "Inscripcion.MateriaNoEncontrada",
        $"No existe la materia {materiaId}.");

    /// <summary>
    /// Crea el error para una materia que el estudiante no tiene inscrita.
    /// </summary>
    /// <param name="materiaId">Materia buscada.</param>
    /// <returns>El error de recurso no encontrado.</returns>
    public static Error CrearMateriaNoInscrita(int materiaId) => Error.CrearNoEncontrado(
        "Inscripcion.MateriaNoInscrita",
        $"No tiene inscrita la materia {materiaId}.");

    /// <summary>
    /// Crea el error para consultar los compañeros de una clase en la que el estudiante no está inscrito (R9).
    /// </summary>
    /// <returns>El error de falta de permiso.</returns>
    public static Error CrearCompanerosNoPermitidos() => Error.CrearProhibido(
        "Inscripcion.CompanerosNoPermitidos",
        "Solo puede ver los compañeros de las materias que tiene inscritas.");

    /// <summary>
    /// Convierte el motivo de rechazo de <see cref="PoliticaInscripcion"/> en un error para el usuario.
    /// </summary>
    /// <param name="motivo">Motivo del rechazo.</param>
    /// <returns>El error correspondiente.</returns>
    public static Error CrearRechazo(MotivoRechazoInscripcion motivo) => motivo switch
    {
        MotivoRechazoInscripcion.MateriaInactiva => Error.CrearValidacion(
            "Inscripcion.MateriaInactiva", "La materia no está disponible para inscripción.", CrearDetalleMateria("La materia está desactivada.")),
        MotivoRechazoInscripcion.MateriaDeOtroPrograma => Error.CrearValidacion(
            "Inscripcion.MateriaDeOtroPrograma", "La materia no pertenece a su programa.", CrearDetalleMateria("Solo puede inscribir materias de su programa.")),
        MotivoRechazoInscripcion.MateriaYaInscrita => Error.CrearConflicto(
            "Inscripcion.MateriaYaInscrita", "Ya tiene inscrita esta materia."),
        MotivoRechazoInscripcion.MaximoMateriasAlcanzado => Error.CrearConflicto(
            "Inscripcion.MaximoMaterias", $"Ya tiene inscritas {PoliticaInscripcion.MaximoMaterias} materias, el máximo permitido."),
        MotivoRechazoInscripcion.MaximoCreditosSuperado => Error.CrearConflicto(
            "Inscripcion.MaximoCreditos", $"La materia supera el máximo de {PoliticaInscripcion.MaximoCreditos} créditos del programa."),
        MotivoRechazoInscripcion.ProfesorRepetido => Error.CrearConflicto(
            "Inscripcion.ProfesorRepetido", "Ya tiene otra materia con el mismo profesor; no puede tomar dos clases con él."),
        _ => Error.CrearFalla("Inscripcion.Rechazada", "La inscripción no cumple las reglas.")
    };

    /// <summary>
    /// Crea el detalle por campo para el identificador de la materia.
    /// </summary>
    /// <param name="mensaje">Mensaje del campo.</param>
    /// <returns>El detalle del campo <c>MateriaId</c>.</returns>
    private static Dictionary<string, string[]> CrearDetalleMateria(string mensaje) =>
        new(StringComparer.Ordinal) { ["MateriaId"] = [mensaje] };
}
