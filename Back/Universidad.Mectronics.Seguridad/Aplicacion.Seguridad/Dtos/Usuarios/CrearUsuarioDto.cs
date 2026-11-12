namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Datos para crear una cuenta. Los datos académicos dependen del tipo de perfil del rol.
/// </summary>
/// <param name="NombreCompleto">Nombre completo.</param>
/// <param name="Identificacion">Documento de identidad.</param>
/// <param name="Email">Correo de inicio de sesión.</param>
/// <param name="RolId">Rol que se asignará.</param>
/// <param name="ProgramaId">Programa académico; solo para roles con perfil de estudiante.</param>
/// <param name="UbicacionSemestral">Semestre que cursa, entre 1 y 10; solo para roles con perfil de estudiante.</param>
/// <param name="CategoriaId">Categoría de vinculación; solo para roles con perfil de profesor.</param>
public sealed record CrearUsuarioDto(
    string? NombreCompleto,
    string? Identificacion,
    string? Email,
    byte? RolId,
    int? ProgramaId,
    int? UbicacionSemestral,
    byte? CategoriaId);
