using System.Security.Cryptography;
using System.Text;
using Api.Seguridad.Errores;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Seguridad.Csrf;

/// <summary>
/// Valida el token CSRF (double-submit cookie): en las peticiones que modifican datos, la cabecera <c>X-CSRF-TOKEN</c>
/// debe coincidir con la cookie <c>universidad-csrf</c>.
/// </summary>
/// <param name="siguiente">Siguiente componente del pipeline.</param>
public sealed class MiddlewareCsrf(RequestDelegate siguiente)
{
    /// <summary>
    /// Valida el token contra CSRF cuando corresponde y continúa el pipeline.
    /// </summary>
    /// <param name="contexto">Contexto de la petición.</param>
    /// <returns>Una tarea que termina cuando la petición se atiende.</returns>
    public async Task InvokeAsync(HttpContext contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto);
        if (RequiereValidacion(contexto) && !TieneTokenValido(contexto.Request))
        {
            await ErroresApi.CrearCsrfInvalido().ConvertirAProblema().ExecuteAsync(contexto);
            return;
        }

        await siguiente(contexto);
    }

    /// <summary>
    /// Indica si la petición debe validarse: modifica datos, trae cookies de sesión y el endpoint no está marcado con
    /// <see cref="OmitirValidacionCsrfAttribute"/>.
    /// </summary>
    /// <param name="contexto">Contexto de la petición.</param>
    /// <returns><see langword="true"/> si se debe exigir el token.</returns>
    private static bool RequiereValidacion(HttpContext contexto)
    {
        string metodo = contexto.Request.Method;
        bool modificaDatos = !(HttpMethods.IsGet(metodo) || HttpMethods.IsHead(metodo) || HttpMethods.IsOptions(metodo) || HttpMethods.IsTrace(metodo));
        bool traeCookiesSesion = contexto.Request.Cookies.ContainsKey(NombresCookie.Acceso) ||
                                 contexto.Request.Cookies.ContainsKey(NombresCookie.Renovacion);
        bool omitida = contexto.GetEndpoint()?.Metadata.GetMetadata<OmitirValidacionCsrfAttribute>() is not null;
        return modificaDatos && traeCookiesSesion && !omitida;
    }

    /// <summary>
    /// Compara en tiempo constante la cookie y la cabecera.
    /// </summary>
    /// <param name="solicitud">Petición en curso.</param>
    /// <returns><see langword="true"/> si ambas existen y son iguales.</returns>
    private static bool TieneTokenValido(HttpRequest solicitud)
    {
        string? cookie = solicitud.Cookies[NombresCookie.Csrf];
        string? cabecera = solicitud.Headers[NombresCookie.CabeceraCsrf];
        return !string.IsNullOrEmpty(cookie) &&
               !string.IsNullOrEmpty(cabecera) &&
               CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(cookie), Encoding.UTF8.GetBytes(cabecera));
    }
}
