namespace Aplicacion.Seguridad.Dtos.Autenticacion;

/// <summary>
/// Token firmado (de acceso o de renovación) y su fecha de vencimiento. Lo genera <c>IServicioTokens</c>
/// y la API lo entrega en una cookie <c>HttpOnly</c>.
/// </summary>
/// <param name="Valor">Token firmado, listo para enviarse en la cookie.</param>
/// <param name="VenceUtc">Fecha y hora (UTC) en que el token deja de ser válido.</param>
public sealed record TokenEmitidoDto(string Valor, DateTimeOffset VenceUtc);
