using Dominio.Academico.Politicas;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Errores;

/// <summary>
/// Errores esperados de la administración de materias.
/// </summary>
public static class ErroresMateria
{
    /// <summary>
    /// Crea el error para datos de entrada con formato inválido.
    /// </summary>
    /// <param name="detalles">Mensajes de error por campo.</param>
    /// <returns>El error de validación.</returns>
    public static Error CrearDatosInvalidos(IReadOnlyDictionary<string, string[]> detalles) => Error.CrearValidacion(
        "Materia.DatosInvalidos",
        "La materia tiene datos inválidos.",
        detalles);

    /// <summary>
    /// Crea el error para una materia que no existe.
    /// </summary>
    /// <param name="id">Materia buscada.</param>
    /// <returns>El error de recurso no encontrado.</returns>
    public static Error CrearNoEncontrada(int id) => Error.CrearNoEncontrado(
        "Materia.NoEncontrada",
        $"No existe la materia {id}.");

    /// <summary>
    /// Crea el error para consultar materias inactivas sin el permiso de administración.
    /// </summary>
    /// <returns>El error de falta de permiso.</returns>
    public static Error CrearSinPermisoInactivas() => Error.CrearProhibido(
        "Materia.SinPermiso",
        "No tiene permiso para consultar las materias desactivadas.");

    /// <summary>
    /// Crea el error para un programa inexistente o inactivo.
    /// </summary>
    /// <returns>El error de validación del campo <c>ProgramaId</c>.</returns>
    public static Error CrearProgramaNoValido() => Error.CrearValidacion(
        "Materia.DatosInvalidos",
        "La materia tiene datos inválidos.",
        new Dictionary<string, string[]>(StringComparer.Ordinal) { ["ProgramaId"] = ["El programa no existe o está inactivo."] });

    /// <summary>
    /// Crea el error para un profesor inexistente o con la cuenta inactiva.
    /// </summary>
    /// <returns>El error de validación del campo <c>ProfesorId</c>.</returns>
    public static Error CrearProfesorNoValido() => Error.CrearValidacion(
        "Materia.DatosInvalidos",
        "La materia tiene datos inválidos.",
        new Dictionary<string, string[]>(StringComparer.Ordinal) { ["ProfesorId"] = ["El profesor no existe o su cuenta está inactiva."] });

    /// <summary>
    /// Crea el error para un nombre que ya usa otra materia del programa.
    /// </summary>
    /// <param name="nombre">Nombre repetido.</param>
    /// <returns>El error de conflicto.</returns>
    public static Error CrearNombreDuplicado(string nombre) => Error.CrearConflicto(
        "Materia.NombreDuplicado",
        $"El programa ya tiene una materia llamada '{nombre}'.");

    /// <summary>
    /// Crea el error para un profesor que ya dicta el máximo de materias (R6).
    /// </summary>
    /// <returns>El error de conflicto.</returns>
    public static Error CrearProfesorSinCupo() => Error.CrearConflicto(
        "Materia.ProfesorSinCupo",
        $"El profesor ya dicta {PoliticaMateria.MaximoMateriasPorProfesor} materias, el máximo permitido.");

    /// <summary>
    /// Crea el error para desactivar una materia con estudiantes inscritos.
    /// </summary>
    /// <param name="inscritos">Número de estudiantes inscritos.</param>
    /// <returns>El error de conflicto.</returns>
    public static Error CrearConInscritos(int inscritos)
    {
        string detalle = inscritos == 1
            ? "1 estudiante inscrito; debe cancelar su inscripción"
            : $"{inscritos} estudiantes inscritos; deben cancelar su inscripción";
        return Error.CrearConflicto(
            "Materia.ConInscritos",
            $"La materia tiene {detalle} antes de desactivarla.");
    }

    /// <summary>
    /// Crea el error para cambiar el profesor de una materia con estudiantes inscritos: podría dejar a un estudiante con dos
    /// materias del mismo profesor (R7).
    /// </summary>
    /// <returns>El error de conflicto.</returns>
    public static Error CrearCambioProfesorConInscritos() => Error.CrearConflicto(
        "Materia.CambioProfesorConInscritos",
        "No se puede cambiar el profesor de una materia con estudiantes inscritos.");
}
