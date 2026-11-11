using Aplicacion.Seguridad.Dtos.Autenticacion;

namespace Api.Seguridad.Dtos;

/// <summary>
/// Respuesta HTTP al iniciar sesión, renovarla o cambiar la contraseña. Los tokens no van aquí: viajan en cookies
/// <c>HttpOnly</c> que JavaScript no puede leer; el frontend solo recibe los datos de la cuenta y los vencimientos.
/// </summary>
/// <param name="Usuario">Datos de la cuenta autenticada.</param>
/// <param name="VenceAccesoUtc">Fecha y hora (UTC) en que vence el token de acceso; antes de esa hora conviene renovar.</param>
/// <param name="VenceRenovacionUtc">Fecha y hora (UTC) en que vence la sesión si no se renueva.</param>
public sealed record RespuestaSesionDto(
    UsuarioSesionDto Usuario,
    DateTimeOffset VenceAccesoUtc,
    DateTimeOffset VenceRenovacionUtc);
