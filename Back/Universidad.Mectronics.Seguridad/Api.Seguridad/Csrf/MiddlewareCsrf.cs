using System.Security.Cryptography;
using System.Text;
using Api.Seguridad.Errores;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Seguridad.Csrf;

/// <summary>
/// Protección contra CSRF con el patrón double-submit cookie. Como el navegador envía las cookies de sesión de forma
/// automática, un sitio malicioso podría provocar una petición en nombre del usuario. Para impedirlo, toda petición que
/// modifica datos con cookies de sesión debe traer en la cabecera <c>X-CSRF-TOKEN</c> el mismo valor de la cookie
/// <c>universidad-csrf</c>: un sitio ajeno no puede leer esa cookie, así que no puede copiar el valor. Se complementa con
/// <c>SameSite=Strict</c> en las cookies.
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
    /// Indica si la petición debe validarse: modifica datos, trae cookies de sesión (las que el navegador envía solo) y el
    /// endpoint no está marcado con <see cref="OmitirValidacionCsrfAttribute"/>. Sin cookies de sesión no hay credenciales que abusar.
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
