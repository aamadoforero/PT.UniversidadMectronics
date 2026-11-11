namespace Api.Seguridad.Csrf;

/// <summary>
/// Marca una acción que modifica datos pero no necesita el token contra CSRF. Solo aplica al inicio de sesión, que todavía
/// no tiene una sesión que proteger. Se usa como <c>[OmitirValidacionCsrf]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class OmitirValidacionCsrfAttribute : Attribute;
