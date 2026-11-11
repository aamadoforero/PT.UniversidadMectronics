namespace Infraestructura.Seguridad.Persistencia.Modelos;

/// <summary>
/// Fila de solo lectura de <c>Academico.CategoriaProfesor</c>. No es una entidad del dominio: Seguridad solo la consulta para
/// llenar listas y verificar que una categoría exista.
/// </summary>
internal sealed class CategoriaProfesorCatalogo
{
    /// <summary>Identificador de la categoría.</summary>
    public byte Id { get; private set; }

    /// <summary>Nombre de la categoría.</summary>
    public string Nombre { get; private set; } = string.Empty;
}
