namespace Universidad.Mectronics.Transversal.Auditoria;

/// <summary>
/// Cabeceras HTTP de la comunicación entre microservicios.
/// </summary>
public static class NombresCabecera
{
    /// <summary>Clave con la que cada microservicio se identifica ante el de Logs.</summary>
    public const string ClaveAplicacion = "X-Api-Key";
}
