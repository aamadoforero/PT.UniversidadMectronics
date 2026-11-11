namespace Api.Seguridad.Configuracion;

/// <summary>
/// Configuración de las cookies de sesión (sección <c>Sesion</c>). No contiene secretos: va en los archivos appsettings.
/// </summary>
public sealed class OpcionesSesion
{
    /// <summary>Nombre de la sección de configuración.</summary>
    public const string Seccion = "Sesion";

    /// <summary>
    /// Indica si las cookies solo viajan por HTTPS. Debe ser <see langword="true"/> en cualquier servidor; solo se desactiva
    /// en desarrollo local por HTTP.
    /// </summary>
    public bool CookieSegura { get; set; } = true;

    /// <summary>
    /// Política <c>SameSite</c> de las cookies. <c>Strict</c> impide que otro sitio las envíe, lo que refuerza la protección
    /// contra CSRF; <c>None</c> solo se permite con <see cref="CookieSegura"/>.
    /// </summary>
    public SameSiteMode MismoSitio { get; set; } = SameSiteMode.Strict;

    /// <summary>Ruta a la que se limita la cookie de renovación, para que no viaje en el resto de peticiones.</summary>
    public string RutaRenovacion { get; set; } = "/api/seguridad/sesion";
}
