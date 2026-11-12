namespace Api.Gateway.Configuracion;

/// <summary>
/// Límite global de solicitudes por dirección IP (sección <c>LimiteSolicitudes</c>).
/// </summary>
public sealed class OpcionesLimiteSolicitudes
{
    /// <summary>Nombre de la sección de configuración.</summary>
    public const string Seccion = "LimiteSolicitudes";

    /// <summary>Solicitudes permitidas por IP en cada ventana de un minuto.</summary>
    public int PermitidasPorMinuto { get; set; } = 300;
}
