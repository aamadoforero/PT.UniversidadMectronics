namespace Dominio.Academico.Autorizacion;

/// <summary>
/// Códigos de permiso de las inscripciones. Coinciden con <c>Seguridad.Permiso</c> en <c>Database/DML.sql</c>.
/// </summary>
public static class CodigosPermisoInscripciones
{
    /// <summary>Inscribir y cancelar las materias propias y ver los compañeros de cada clase.</summary>
    public const string Propias = "inscripciones.propias";

    /// <summary>Ver las inscripciones de los estudiantes.</summary>
    public const string Consultar = "inscripciones.consultar";
}
