using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Api.Logs.Errores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Universidad.Mectronics.Transversal.Auditoria;

namespace Api.Logs.Autenticacion;

/// <summary>
/// Autentica a la aplicación que llama con la clave de la cabecera <see cref="NombresCabecera.ClaveAplicacion"/>.
/// </summary>
/// <param name="opciones">Opciones del esquema.</param>
/// <param name="registro">Fábrica de logs.</param>
/// <param name="codificador">Codificador de URL.</param>
/// <param name="claves">Claves de las aplicaciones autorizadas.</param>
public sealed class ManejadorClaveAplicacion(
    IOptionsMonitor<AuthenticationSchemeOptions> opciones,
    ILoggerFactory registro,
    UrlEncoder codificador,
    IOptions<OpcionesClavesAplicacion> claves) : AuthenticationHandler<AuthenticationSchemeOptions>(opciones, registro, codificador)
{
    /// <summary>
    /// Busca la aplicación dueña de la clave recibida.
    /// </summary>
    /// <returns>Éxito con la identidad de la aplicación, sin resultado si no hay cabecera, o fallo si la clave no es válida.</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(NombresCabecera.ClaveAplicacion, out StringValues valores) || StringValues.IsNullOrEmpty(valores))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string? aplicacion = BuscarAplicacion(valores.ToString());
        if (aplicacion is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("La clave de aplicación no es válida."));
        }

        ClaimsIdentity identidad = new([new Claim(ClaimTypes.Name, aplicacion)], Scheme.Name);
        AuthenticationTicket ticket = new(new ClaimsPrincipal(identidad), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <summary>
    /// Responde 401 con Problem Details cuando falta la clave o no es válida.
    /// </summary>
    /// <param name="properties">Propiedades de la autenticación; no se usan.</param>
    /// <returns>Una tarea que termina cuando se escribe la respuesta.</returns>
    protected override Task HandleChallengeAsync(AuthenticationProperties properties) =>
        ErroresApi.CrearClaveInvalida().ConvertirAProblema().ExecuteAsync(Context);

    /// <summary>Busca la aplicación dueña de la clave comparando en tiempo constante.</summary>
    /// <param name="clave">Clave recibida.</param>
    /// <returns>El nombre de la aplicación, o <see langword="null"/> si ninguna coincide.</returns>
    private string? BuscarAplicacion(string clave)
    {
        byte[] huellaRecibida = SHA256.HashData(Encoding.UTF8.GetBytes(clave));
        string? encontrada = null;
        foreach (KeyValuePair<string, string> entrada in claves.Value)
        {
            byte[] huellaConfigurada = SHA256.HashData(Encoding.UTF8.GetBytes(entrada.Value));
            if (CryptographicOperations.FixedTimeEquals(huellaRecibida, huellaConfigurada))
            {
                encontrada = entrada.Key;
            }
        }

        return encontrada;
    }
}
