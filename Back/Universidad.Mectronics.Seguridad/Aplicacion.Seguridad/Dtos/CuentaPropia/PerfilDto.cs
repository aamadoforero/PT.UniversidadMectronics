using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Dtos.CuentaPropia;

/// <summary>
/// Perfil completo de la cuenta autenticada: datos de la cuenta, rol, permisos y datos académicos según su tipo de perfil.
/// </summary>
/// <param name="Id">Identificador de la cuenta.</param>
/// <param name="NombreCompleto">Nombre completo; es el único dato que el usuario puede editar.</param>
/// <param name="Identificacion">Documento de identidad.</param>
/// <param name="Email">Correo de inicio de sesión.</param>
/// <param name="Rol">Nombre del rol.</param>
/// <param name="TipoPerfil">Datos académicos que exige el rol.</param>
/// <param name="Permisos">Códigos de los permisos del rol.</param>
/// <param name="DebeCambiarContrasena">Indica que la contraseña es temporal y debe cambiarse.</param>
/// <param name="PerfilEstudiante">Datos de estudiante; <see langword="null"/> si el rol no tiene ese perfil.</param>
/// <param name="PerfilProfesor">Datos de profesor; <see langword="null"/> si el rol no tiene ese perfil.</param>
public sealed record PerfilDto(
    int Id,
    string NombreCompleto,
    string Identificacion,
    string Email,
    string Rol,
    TipoPerfil TipoPerfil,
    IReadOnlyList<string> Permisos,
    bool DebeCambiarContrasena,
    PerfilEstudianteDto? PerfilEstudiante,
    PerfilProfesorDto? PerfilProfesor);
