namespace Dominio.Academico.Autorizacion;

/// <summary>
/// Todos los permisos que usa el microservicio Académico. La API crea una política de autorización por cada uno.
/// </summary>
public static class CodigosPermiso
{
    /// <summary>Lista completa de los códigos de permiso del microservicio.</summary>
    public static IReadOnlyList<string> Todos { get; } =
    [
        CodigosPermisoMaterias.Gestionar,
        CodigosPermisoMaterias.Dictadas,
        CodigosPermisoInscripciones.Propias,
        CodigosPermisoInscripciones.Consultar
    ];
}
