using Infraestructura.Seguridad.Criptografia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Api.Seguridad.Controllers;

/// <summary>
/// Clave pública de firma (JWKS, RFC 7517) en <c>/.well-known/jwks.json</c>. Los demás microservicios la descargan para
/// verificar los tokens de acceso sin tener la clave privada.
/// </summary>
/// <param name="proveedorClaves">Proveedor de las claves de firma.</param>
[Route(".well-known")]
[Tags("Claves")]
[AllowAnonymous]
public sealed class ClavesController(ProveedorClavesJwt proveedorClaves) : SeguridadControllerBase
{
    /// <summary>
    /// Devuelve las claves públicas en formato JWKS. Solo incluye los parámetros públicos (n, e); nunca la parte privada.
    /// </summary>
    /// <returns>200 con el conjunto de claves.</returns>
    [HttpGet("jwks.json")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ObtenerClavesPublicas()
    {
        JsonWebKeySet conjunto = proveedorClaves.ObtenerConjuntoClavesPublicas();
        return Ok(new
        {
            keys = conjunto.Keys.Select(clave => new { kty = clave.Kty, use = clave.Use, alg = clave.Alg, kid = clave.Kid, n = clave.N, e = clave.E })
        });
    }
}
