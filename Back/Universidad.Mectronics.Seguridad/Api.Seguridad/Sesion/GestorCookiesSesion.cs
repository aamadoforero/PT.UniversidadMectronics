using System.Security.Cryptography;
using Api.Seguridad.Configuracion;
using Api.Seguridad.Dtos;
using Aplicacion.Seguridad.Dtos.Autenticacion;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Seguridad.Sesion;

/// <summary>
/// Escribe y elimina las tres cookies de sesión: la de acceso y la de renovación (<c>HttpOnly</c>, con los tokens) y la de
/// CSRF (legible por JavaScript, con un valor aleatorio nuevo en cada sesión).
/// </summary>
/// <param name="opciones">Configuración de las cookies.</param>
public sealed class GestorCookiesSesion(IOptions<OpcionesSesion> opciones)
{
    /// <summary>Bytes aleatorios del token contra CSRF (256 bits).</summary>
    private const int BytesTokenCsrf = 32;

    /// <summary>
    /// Escribe las cookies de una sesión nueva o renovada y arma la respuesta para el frontend.
    /// </summary>
    /// <param name="respuesta">Respuesta HTTP en curso.</param>
    /// <param name="sesion">Sesión creada por la capa de Aplicación.</param>
    /// <returns>Los datos de la cuenta y los vencimientos, sin los tokens.</returns>
    public RespuestaSesionDto EscribirSesion(HttpResponse respuesta, SesionCreadaDto sesion)
    {
        ArgumentNullException.ThrowIfNull(respuesta);
        ArgumentNullException.ThrowIfNull(sesion);
        OpcionesSesion configuracion = opciones.Value;

        respuesta.Cookies.Append(NombresCookie.Acceso, sesion.TokenAcceso.Valor,
            CrearOpciones(httpOnly: true, "/", sesion.TokenAcceso.VenceUtc));
        respuesta.Cookies.Append(NombresCookie.Renovacion, sesion.TokenRenovacion.Valor,
            CrearOpciones(httpOnly: true, configuracion.RutaRenovacion, sesion.TokenRenovacion.VenceUtc));
        respuesta.Cookies.Append(NombresCookie.Csrf, GenerarTokenCsrf(),
            CrearOpciones(httpOnly: false, "/", sesion.TokenRenovacion.VenceUtc));

        return new RespuestaSesionDto(sesion.Usuario, sesion.TokenAcceso.VenceUtc, sesion.TokenRenovacion.VenceUtc);
    }

    /// <summary>
    /// Elimina las tres cookies de sesión (al cerrar sesión o cuando la renovación falla).
    /// </summary>
    /// <param name="respuesta">Respuesta HTTP en curso.</param>
    public void EliminarSesion(HttpResponse respuesta)
    {
        ArgumentNullException.ThrowIfNull(respuesta);
        respuesta.Cookies.Delete(NombresCookie.Acceso, CrearOpciones(httpOnly: true, "/", null));
        respuesta.Cookies.Delete(NombresCookie.Renovacion, CrearOpciones(httpOnly: true, opciones.Value.RutaRenovacion, null));
        respuesta.Cookies.Delete(NombresCookie.Csrf, CrearOpciones(httpOnly: false, "/", null));
    }

    /// <summary>
    /// Arma las opciones comunes de una cookie de sesión.
    /// </summary>
    /// <param name="httpOnly">Indica si JavaScript no debe poder leerla.</param>
    /// <param name="ruta">Ruta a la que se limita la cookie.</param>
    /// <param name="vence">Vencimiento; <see langword="null"/> al eliminarla.</param>
    /// <returns>Las opciones de la cookie.</returns>
    private CookieOptions CrearOpciones(bool httpOnly, string ruta, DateTimeOffset? vence) => new()
    {
        HttpOnly = httpOnly,
        Secure = opciones.Value.CookieSegura,
        SameSite = opciones.Value.MismoSitio,
        Path = ruta,
        Expires = vence,
        IsEssential = true
    };

    /// <summary>
    /// Genera un valor aleatorio criptográficamente seguro para el token contra CSRF.
    /// </summary>
    /// <returns>El token en Base64Url.</returns>
    private static string GenerarTokenCsrf() => Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(BytesTokenCsrf));
}
