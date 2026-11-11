using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Dtos.CuentaPropia;
using Aplicacion.Seguridad.Dtos.Usuarios;
using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Mapeos;

/// <summary>
/// Conversiones de la entidad <see cref="Usuario"/> a los DTOs que devuelve Aplicación. Concentra el mapeo en un solo lugar
/// para que los DTOs sigan siendo objetos sin comportamiento y los servicios no repitan la conversión.
/// </summary>
public static class ExtensionesMapeoUsuario
{
    /// <summary>
    /// Convierte la cuenta en los datos de sesión que recibe el frontend.
    /// </summary>
    /// <param name="usuario">Cuenta con su rol y permisos cargados.</param>
    /// <returns>Los datos de la cuenta autenticada.</returns>
    public static UsuarioSesionDto ConvertirAUsuarioSesionDto(this Usuario usuario) => new(
        usuario.Id,
        usuario.NombreCompleto.Valor,
        usuario.Email.Valor,
        usuario.Rol.Nombre,
        ObtenerCodigosPermiso(usuario),
        usuario.DebeCambiarContrasena);

    /// <summary>
    /// Convierte la cuenta en su perfil completo, con los datos académicos que correspondan a su tipo de perfil.
    /// </summary>
    /// <param name="usuario">Cuenta con su rol, permisos y perfil académico cargados.</param>
    /// <returns>El perfil de la cuenta.</returns>
    public static PerfilDto ConvertirAPerfilDto(this Usuario usuario) => new(
        usuario.Id,
        usuario.NombreCompleto.Valor,
        usuario.Identificacion.Valor,
        usuario.Email.Valor,
        usuario.Rol.Nombre,
        usuario.Rol.TipoPerfil,
        ObtenerCodigosPermiso(usuario),
        usuario.DebeCambiarContrasena,
        ConvertirPerfilEstudiante(usuario),
        ConvertirPerfilProfesor(usuario));

    /// <summary>
    /// Convierte la cuenta en una fila de listado para la administración.
    /// </summary>
    /// <param name="usuario">Cuenta con su rol cargado.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC, para calcular si está bloqueada.</param>
    /// <returns>El resumen de la cuenta.</returns>
    public static UsuarioResumenDto ConvertirAUsuarioResumenDto(this Usuario usuario, DateTimeOffset ahoraUtc) => new(
        usuario.Id,
        usuario.NombreCompleto.Valor,
        usuario.Identificacion.Valor,
        usuario.Email.Valor,
        usuario.Rol.Nombre,
        usuario.Activo,
        usuario.EstaBloqueado(ahoraUtc),
        usuario.DebeCambiarContrasena);

    /// <summary>
    /// Convierte la cuenta en su detalle completo para la administración.
    /// </summary>
    /// <param name="usuario">Cuenta con su rol y perfil académico cargados.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC, para calcular si está bloqueada.</param>
    /// <returns>El detalle de la cuenta.</returns>
    public static UsuarioDetalleDto ConvertirAUsuarioDetalleDto(this Usuario usuario, DateTimeOffset ahoraUtc) => new(
        usuario.Id,
        usuario.NombreCompleto.Valor,
        usuario.Identificacion.Valor,
        usuario.Email.Valor,
        usuario.RolId,
        usuario.Rol.Nombre,
        usuario.Rol.TipoPerfil,
        usuario.Activo,
        usuario.DebeCambiarContrasena,
        usuario.EstaBloqueado(ahoraUtc),
        usuario.BloqueoHasta,
        usuario.FechaCreacion,
        usuario.FechaModificacion,
        ConvertirPerfilEstudiante(usuario),
        ConvertirPerfilProfesor(usuario));

    /// <summary>
    /// Obtiene los códigos de permiso del rol de la cuenta, ordenados alfabéticamente.
    /// </summary>
    /// <param name="usuario">Cuenta con su rol y permisos cargados.</param>
    /// <returns>Los códigos de permiso.</returns>
    private static IReadOnlyList<string> ObtenerCodigosPermiso(Usuario usuario) =>
        usuario.Rol.Permisos.Select(permiso => permiso.Codigo).Order(StringComparer.Ordinal).ToList();

    /// <summary>
    /// Convierte el perfil de estudiante de la cuenta, si lo tiene.
    /// </summary>
    /// <param name="usuario">Cuenta con su perfil académico cargado.</param>
    /// <returns>Los datos de estudiante, o <see langword="null"/> si la cuenta no tiene ese perfil.</returns>
    private static PerfilEstudianteDto? ConvertirPerfilEstudiante(Usuario usuario) =>
        usuario.PerfilEstudiante is null
            ? null
            : new PerfilEstudianteDto(usuario.PerfilEstudiante.ProgramaId, usuario.PerfilEstudiante.UbicacionSemestral);

    /// <summary>
    /// Convierte el perfil de profesor de la cuenta, si lo tiene.
    /// </summary>
    /// <param name="usuario">Cuenta con su perfil académico cargado.</param>
    /// <returns>Los datos de profesor, o <see langword="null"/> si la cuenta no tiene ese perfil.</returns>
    private static PerfilProfesorDto? ConvertirPerfilProfesor(Usuario usuario) =>
        usuario.PerfilProfesor is null
            ? null
            : new PerfilProfesorDto(usuario.PerfilProfesor.CategoriaId);
}
