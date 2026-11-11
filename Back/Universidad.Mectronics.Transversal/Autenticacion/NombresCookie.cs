namespace Universidad.Mectronics.Transversal.Autenticacion;

/// <summary>
/// Nombres de las cookies de sesión. Seguridad las escribe y todos los microservicios leen la de acceso, así que deben
/// coincidir en ambos lados.
/// </summary>
public static class NombresCookie
{
    /// <summary>Token de acceso. Es <c>HttpOnly</c>: JavaScript no puede leerla, lo que la protege de ataques XSS.</summary>
    public const string Acceso = "universidad-acceso";

    /// <summary>Token de renovación. Es <c>HttpOnly</c> y solo viaja a las rutas de sesión de Seguridad.</summary>
    public const string Renovacion = "universidad-renovacion";

    /// <summary>Token contra CSRF. JavaScript la lee y la reenvía en la cabecera <see cref="CabeceraCsrf"/>.</summary>
    public const string Csrf = "universidad-csrf";

    /// <summary>Cabecera en la que el frontend reenvía el valor de la cookie <see cref="Csrf"/>.</summary>
    public const string CabeceraCsrf = "X-CSRF-TOKEN";
}
