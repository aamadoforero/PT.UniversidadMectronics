using Aplicacion.Seguridad.Dtos.CuentaPropia;
using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Datos completos de una cuenta para la vista de administración.
/// </summary>
/// <param name="Id">Identificador de la cuenta.</param>
/// <param name="NombreCompleto">Nombre completo.</param>
/// <param name="Identificacion">Documento de identidad.</param>
/// <param name="Email">Correo de inicio de sesión.</param>
/// <param name="RolId">Identificador del rol.</param>
/// <param name="Rol">Nombre del rol.</param>
/// <param name="TipoPerfil">Datos académicos que exige el rol.</param>
/// <param name="Activo">Indica si la cuenta está habilitada.</param>
/// <param name="DebeCambiarContrasena">Indica que la contraseña es temporal.</param>
/// <param name="EstaBloqueado">Indica si la cuenta está bloqueada en este momento por intentos fallidos.</param>
/// <param name="BloqueoHasta">Fecha y hora (UTC) en que termina el bloqueo; <see langword="null"/> si no hay bloqueo.</param>
/// <param name="FechaCreacion">Fecha y hora (UTC) de creación.</param>
/// <param name="FechaModificacion">Fecha y hora (UTC) del último cambio de datos.</param>
/// <param name="PerfilEstudiante">Datos de estudiante; <see langword="null"/> si el rol no tiene ese perfil.</param>
/// <param name="PerfilProfesor">Datos de profesor; <see langword="null"/> si el rol no tiene ese perfil.</param>
public sealed record UsuarioDetalleDto(
    int Id,
    string NombreCompleto,
    string Identificacion,
    string Email,
    byte RolId,
    string Rol,
    TipoPerfil TipoPerfil,
    bool Activo,
    bool DebeCambiarContrasena,
    bool EstaBloqueado,
    DateTimeOffset? BloqueoHasta,
    DateTimeOffset FechaCreacion,
    DateTimeOffset? FechaModificacion,
    PerfilEstudianteDto? PerfilEstudiante,
    PerfilProfesorDto? PerfilProfesor);
