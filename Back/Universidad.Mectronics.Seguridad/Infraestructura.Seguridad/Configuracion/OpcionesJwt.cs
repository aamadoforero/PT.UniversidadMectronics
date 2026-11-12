namespace Infraestructura.Seguridad.Configuracion;

/// <summary>
/// Configuración de los tokens de sesión (sección <c>Jwt</c>). Se valida con <see cref="ValidadorOpcionesJwt"/> al
/// arrancar.
/// </summary>
public sealed class OpcionesJwt
{
    /// <summary>Nombre de la sección de configuración.</summary>
    public const string Seccion = "Jwt";

    /// <summary>Emisor de los tokens (claim <c>iss</c>). No es secreto.</summary>
    public string Emisor { get; set; } = string.Empty;

    /// <summary>Destinatario de los tokens de acceso (claim <c>aud</c>); lo exigen todos los microservicios. No es secreto.</summary>
    public string Audiencia { get; set; } = string.Empty;

    /// <summary>Minutos de vigencia del token de acceso, entre 1 y 60. No es secreto.</summary>
    public int MinutosAcceso { get; set; } = 15;

    /// <summary>Días de vigencia del token de renovación, entre 1 y 30. No es secreto.</summary>
    public int DiasRenovacion { get; set; } = 7;

    /// <summary>
    /// Secreto. Clave privada RSA (2048 bits o más) en PKCS#8 y Base64, con la que se firma el token de acceso.
    /// </summary>
    public string ClavePrivadaAcceso { get; set; } = string.Empty;

    /// <summary>Secreto. Al menos 32 bytes en Base64. Firma el token de renovación con HMAC-SHA256.</summary>
    public string ClaveRenovacion { get; set; } = string.Empty;

    /// <summary>Secreto. Al menos 32 bytes en Base64. Calcula la huella de la contraseña con HMAC-SHA256.</summary>
    public string ClaveHuella { get; set; } = string.Empty;
}
