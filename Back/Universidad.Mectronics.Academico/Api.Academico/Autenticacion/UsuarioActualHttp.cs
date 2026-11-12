using System.Globalization;
using System.Security.Claims;
using Aplicacion.Academico.Interfaces;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Academico.Autenticacion;

/// <summary>
/// Implementación de <see cref="IUsuarioActual"/> a partir de los claims del token de acceso, que ya validó el middleware
/// de autenticación. Hay una instancia por petición.
/// </summary>
/// <param name="accesoContexto">Acceso al contexto HTTP de la petición en curso.</param>
public sealed class UsuarioActualHttp(IHttpContextAccessor accesoContexto) : IUsuarioActual
{
    /// <summary>Identidad de la petición en curso, o <see langword="null"/> fuera de una petición.</summary>
    private ClaimsPrincipal? Principal => accesoContexto.HttpContext?.User;

    /// <summary>Indica si la petición trae un token de acceso válido.</summary>
    public bool EstaAutenticado => Principal?.Identity?.IsAuthenticated == true;

    /// <summary>Identificador de la cuenta, tomado del claim <c>sub</c>.</summary>
    public int Id => EstaAutenticado &&
                     int.TryParse(Principal!.FindFirstValue(NombresClaim.UsuarioId), NumberStyles.None, CultureInfo.InvariantCulture, out int id)
        ? id
        : throw new InvalidOperationException("La petición no tiene una cuenta autenticada.");

    /// <summary>
    /// Indica si el token de acceso incluye el permiso.
    /// </summary>
    /// <param name="codigo">Código del permiso.</param>
    /// <returns><see langword="true"/> si la cuenta tiene el permiso.</returns>
    public bool TienePermiso(string codigo) => EstaAutenticado && Principal!.HasClaim(NombresClaim.Permiso, codigo);
}
