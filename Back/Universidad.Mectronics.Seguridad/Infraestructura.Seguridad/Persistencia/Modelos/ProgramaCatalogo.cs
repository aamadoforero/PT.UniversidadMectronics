namespace Infraestructura.Seguridad.Persistencia.Modelos;

/// <summary>
/// Fila de solo lectura de <c>Academico.Programa</c>. No es una entidad del dominio: Seguridad solo la consulta para llenar
/// listas y verificar que un programa exista; el catálogo lo administra el microservicio Académico.
/// </summary>
internal sealed class ProgramaCatalogo
{
    /// <summary>Identificador del programa.</summary>
    public int Id { get; private set; }

    /// <summary>Nombre del programa.</summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>Indica si el programa admite estudiantes.</summary>
    public bool Activo { get; private set; }
}
