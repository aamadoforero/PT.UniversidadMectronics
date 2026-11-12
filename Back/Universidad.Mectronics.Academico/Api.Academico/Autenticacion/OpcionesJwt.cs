namespace Api.Academico.Autenticacion;

/// <summary>
/// Validación del token de acceso emitido por Seguridad (sección <c>Jwt</c>). No hay secretos: Académico solo necesita la
/// clave pública, que descarga del JWKS de Seguridad.
/// </summary>
public sealed class OpcionesJwt
{
    /// <summary>Nombre de la sección de configuración.</summary>
    public const string Seccion = "Jwt";

    /// <summary>Emisor esperado (claim <c>iss</c>); el mismo <c>Jwt:Emisor</c> de Seguridad.</summary>
    public string? Emisor { get; set; }

    /// <summary>Audiencia esperada (claim <c>aud</c>); la misma <c>Jwt:Audiencia</c> de Seguridad.</summary>
    public string? Audiencia { get; set; }

    /// <summary>URL del JWKS de Seguridad, por ejemplo <c>http://localhost:5090/.well-known/jwks.json</c>.</summary>
    public string? UrlClavesPublicas { get; set; }

    /// <summary>Si se exige HTTPS para descargar el JWKS. Solo se desactiva en desarrollo.</summary>
    public bool RequiereHttps { get; set; } = true;
}
