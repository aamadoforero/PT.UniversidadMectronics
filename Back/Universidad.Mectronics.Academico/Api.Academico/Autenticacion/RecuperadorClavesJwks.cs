using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Api.Academico.Autenticacion;

/// <summary>
/// Descarga el JWKS de Seguridad para validar los tokens. <c>ConfigurationManager</c> lo guarda en caché y lo vuelve a
/// descargar cuando llega un <c>kid</c> desconocido.
/// </summary>
internal sealed class RecuperadorClavesJwks : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    /// <summary>
    /// Descarga el JWKS y extrae sus claves de firma.
    /// </summary>
    /// <param name="address">URL del JWKS.</param>
    /// <param name="retriever">Cliente que descarga el documento.</param>
    /// <param name="cancel">Permite cancelar la descarga.</param>
    /// <returns>La configuración con las claves de firma.</returns>
    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(string address, IDocumentRetriever retriever, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(retriever);
        string documento = await retriever.GetDocumentAsync(address, cancel);
        JsonWebKeySet conjunto = new(documento);

        OpenIdConnectConfiguration configuracion = new() { JsonWebKeySet = conjunto };
        foreach (SecurityKey clave in conjunto.GetSigningKeys())
        {
            configuracion.SigningKeys.Add(clave);
        }

        return configuracion;
    }
}
