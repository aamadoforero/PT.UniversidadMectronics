namespace Api.Seguridad.ContrasenaTemporal;

/// <summary>
/// Marca una acción que una sesión con contraseña temporal sí puede usar: ver el perfil, cambiar la contraseña, renovar y
/// cerrar la sesión. Se usa como <c>[PermiteContrasenaTemporal]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class PermiteContrasenaTemporalAttribute : Attribute;
