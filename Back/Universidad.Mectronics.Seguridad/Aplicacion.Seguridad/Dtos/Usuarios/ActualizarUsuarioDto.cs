namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Datos para editar una cuenta. El rol no se cambia aquí; tiene su propia operación porque afecta el acceso.
/// Los datos académicos que se exigen dependen del tipo de perfil del rol actual de la cuenta.
/// </summary>
/// <param name="NombreCompleto">Nombre completo.</param>
/// <param name="Identificacion">Documento de identidad.</param>
/// <param name="Email">Correo de inicio de sesión.</param>
/// <param name="ProgramaId">Programa académico; solo para roles con perfil de estudiante.</param>
/// <param name="UbicacionSemestral">Semestre que cursa, entre 1 y 10; solo para roles con perfil de estudiante.</param>
/// <param name="CategoriaId">Categoría de vinculación; solo para roles con perfil de profesor.</param>
public sealed record ActualizarUsuarioDto(
    string? NombreCompleto,
    string? Identificacion,
    string? Email,
    int? ProgramaId,
    int? UbicacionSemestral,
    byte? CategoriaId);
