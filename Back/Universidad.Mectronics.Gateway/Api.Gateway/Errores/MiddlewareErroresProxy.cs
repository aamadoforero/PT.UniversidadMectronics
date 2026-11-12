using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Model;

namespace Api.Gateway.Errores;

/// <summary>
/// Convierte los fallos del reenvío (servicio caído o tiempo agotado) en una respuesta Problem Details.
/// </summary>
/// <param name="siguiente">Siguiente componente del pipeline del proxy.</param>
/// <param name="registrador">Log del gateway.</param>
public sealed class MiddlewareErroresProxy(RequestDelegate siguiente, ILogger<MiddlewareErroresProxy> registrador)
{
    /// <summary>Clave de los metadatos del clúster con el nombre legible del microservicio.</summary>
    private const string MetadatoNombre = "Nombre";

    /// <summary>
    /// Reenvía la petición y, si el reenvío falló antes de empezar la respuesta, escribe el problema correspondiente.
    /// </summary>
    /// <param name="contexto">Contexto de la petición.</param>
    /// <returns>Una tarea que termina cuando la petición se atiende.</returns>
    public async Task InvokeAsync(HttpContext contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto);
        await siguiente(contexto);

        IForwarderErrorFeature? fallo = contexto.Features.Get<IForwarderErrorFeature>();
        if (fallo is null || contexto.Response.HasStarted)
        {
            return;
        }

        string servicio = ObtenerNombreServicio(contexto);
        registrador.LogWarning(fallo.Exception, "Falló el reenvío a {Servicio} ({Error}) para {Metodo} {Ruta}.",
            servicio, fallo.Error, contexto.Request.Method, contexto.Request.Path);

        IResult respuesta = fallo.Error switch
        {
            ForwarderError.RequestTimedOut => RespuestasError.CrearTiempoAgotado(servicio),
            ForwarderError.Request => RespuestasError.CrearServicioNoDisponible(servicio),
            _ => RespuestasError.CrearErrorComunicacion(servicio)
        };
        await respuesta.ExecuteAsync(contexto);
    }

    /// <summary>
    /// Obtiene el nombre legible del microservicio de destino desde los metadatos de su clúster.
    /// </summary>
    /// <param name="contexto">Contexto de la petición reenviada.</param>
    /// <returns>El nombre del servicio, o el identificador del clúster si no tiene nombre configurado.</returns>
    private static string ObtenerNombreServicio(HttpContext contexto)
    {
        IReverseProxyFeature? proxy = contexto.Features.Get<IReverseProxyFeature>();
        ClusterModel? cluster = proxy?.Cluster;
        if (cluster is null)
        {
            return "solicitado";
        }

        return cluster.Config.Metadata?.GetValueOrDefault(MetadatoNombre) ?? cluster.Config.ClusterId;
    }
}
