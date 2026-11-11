namespace Aplicacion.Seguridad.Dtos.Catalogos;

/// <summary>
/// Elemento de un catálogo de solo lectura (programa académico o categoría de profesor), pensado para llenar
/// listas desplegables en el frontend.
/// </summary>
/// <param name="Id">Identificador del elemento.</param>
/// <param name="Nombre">Nombre visible del elemento.</param>
public sealed record ElementoCatalogoDto(int Id, string Nombre);
