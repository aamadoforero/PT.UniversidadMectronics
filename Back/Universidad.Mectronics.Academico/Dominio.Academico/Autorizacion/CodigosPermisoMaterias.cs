namespace Dominio.Academico.Autorizacion;

/// <summary>
/// Códigos de permiso de la administración de materias. Coinciden con <c>Seguridad.Permiso</c> en <c>Database/DML.sql</c>.
/// </summary>
public static class CodigosPermisoMaterias
{
    /// <summary>Crear, editar, activar y desactivar materias.</summary>
    public const string Gestionar = "materias.gestionar";

    /// <summary>Ver las materias que dicta el profesor autenticado y los estudiantes inscritos en ellas.</summary>
    public const string Dictadas = "materias.dictadas";
}
