namespace Aplicacion.Seguridad.Dtos.CuentaPropia;

/// <summary>
/// Datos académicos de una cuenta cuyo rol tiene perfil de profesor.
/// </summary>
/// <param name="CategoriaId">Categoría de vinculación: Planta, Cátedra u Ocasional.</param>
public sealed record PerfilProfesorDto(byte CategoriaId);
