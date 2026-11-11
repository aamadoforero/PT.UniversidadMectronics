namespace Aplicacion.Seguridad.Dtos.Autenticacion;

/// <summary>
/// Datos que se extraen de un token de renovación cuya firma y vencimiento ya se verificaron. Con ellos, el servicio
/// de autenticación vuelve a consultar la cuenta antes de emitir una sesión nueva.
/// </summary>
/// <param name="UsuarioId">Cuenta a la que pertenece el token.</param>
/// <param name="HuellaContrasena">
/// Huella del hash de la contraseña vigente cuando se emitió el token. Si la contraseña cambió desde entonces,
/// la huella deja de coincidir y el token queda invalidado.
/// </param>
public sealed record TokenRenovacionValidoDto(int UsuarioId, string HuellaContrasena);
