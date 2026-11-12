using Aplicacion.Seguridad.Dtos.Autenticacion;

namespace Api.Seguridad.Dtos;

/// <summary>
/// Respuesta al iniciar sesión, renovarla o cambiar la contraseña. Los tokens viajan en cookies <c>HttpOnly</c>, no en
/// el cuerpo.
/// </summary>
/// <param name="Usuario">Datos de la cuenta autenticada.</param>
/// <param name="VenceAccesoUtc">Fecha y hora (UTC) en que vence el token de acceso; antes de esa hora conviene
/// renovar.</param>
/// <param name="VenceRenovacionUtc">Fecha y hora (UTC) en que vence la sesión si no se renueva.</param>
public sealed record RespuestaSesionDto(
    UsuarioSesionDto Usuario,
    DateTimeOffset VenceAccesoUtc,
    DateTimeOffset VenceRenovacionUtc);
