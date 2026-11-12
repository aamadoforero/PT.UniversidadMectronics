using Aplicacion.Logs.Interfaces;

namespace Api.Logs.Autenticacion;

/// <summary>
/// Implementación de <see cref="IAplicacionActual"/> a partir de la identidad que deja <see cref="ManejadorClaveAplicacion"/>.
/// </summary>
/// <param name="accesor">Acceso al contexto HTTP de la petición.</param>
public sealed class AplicacionActualHttp(IHttpContextAccessor accesor) : IAplicacionActual
{
    /// <summary>Nombre de la aplicación autenticada, o <see langword="null"/> si la petición no tiene una.</summary>
    public string? Nombre => accesor.HttpContext?.User.Identity is { IsAuthenticated: true, Name: string nombre } ? nombre : null;
}
