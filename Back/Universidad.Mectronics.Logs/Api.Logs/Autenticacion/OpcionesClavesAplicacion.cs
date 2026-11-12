namespace Api.Logs.Autenticacion;

/// <summary>
/// Claves de las aplicaciones autorizadas a registrar eventos (sección <c>ClavesAplicacion</c>), por nombre de
/// aplicación.
/// </summary>
public sealed class OpcionesClavesAplicacion : Dictionary<string, string>
{
    /// <summary>Nombre de la sección de configuración.</summary>
    public const string Seccion = "ClavesAplicacion";

    /// <summary>Inicializa una nueva instancia de la clase <see cref="OpcionesClavesAplicacion"/>.</summary>
    public OpcionesClavesAplicacion()
        : base(StringComparer.Ordinal)
    {
    }
}
