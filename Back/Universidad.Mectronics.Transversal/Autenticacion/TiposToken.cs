namespace Universidad.Mectronics.Transversal.Autenticacion;

/// <summary>
/// Valores del encabezado <c>typ</c> de los tokens. Distinguir el tipo impide que un token de renovación se use como token
/// de acceso, o al revés, aunque ambos sean JWT.
/// </summary>
public static class TiposToken
{
    /// <summary>Token de acceso, según el RFC 9068 (<c>at+jwt</c>).</summary>
    public const string Acceso = "at+jwt";

    /// <summary>Token de renovación (<c>rt+jwt</c>).</summary>
    public const string Renovacion = "rt+jwt";
}
