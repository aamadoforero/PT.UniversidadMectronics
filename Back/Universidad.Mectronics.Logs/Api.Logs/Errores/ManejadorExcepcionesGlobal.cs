using Dominio.Logs.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Logs.Errores;

/// <summary>
/// Atrapa las excepciones no controladas, las escribe en el log de la aplicación y responde un Problem Details sin
/// detalles internos.
/// </summary>
/// <param name="registrador">Log de la aplicación.</param>
public sealed class ManejadorExcepcionesGlobal(ILogger<ManejadorExcepcionesGlobal> registrador) : IExceptionHandler
{
    /// <summary>
    /// Clasifica la excepción, la registra si corresponde y escribe la respuesta.
    /// </summary>
    /// <param name="httpContext">Contexto de la petición que falló.</param>
    /// <param name="exception">Excepción no controlada.</param>
    /// <param name="cancellationToken">Cancelación de la petición.</param>
    /// <returns><see langword="true"/>, porque la excepción siempre queda atendida.</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            return true;
        }

        Error error;
        switch (exception)
        {
            case ExcepcionDominio excepcionDominio:
                error = new Error("Dominio.ReglaIncumplida", excepcionDominio.Message, TipoError.Validacion);
                registrador.LogWarning("Regla de dominio incumplida en {Metodo} {Ruta}: {Mensaje}",
                    httpContext.Request.Method, httpContext.Request.Path, excepcionDominio.Message);
                break;
            case BadHttpRequestException:
                error = new Error("Solicitud.Invalida", "La solicitud no tiene un formato válido.", TipoError.Validacion);
                break;
            default:
                error = Error.CrearFalla("Servidor.ErrorInesperado", "Ocurrió un error inesperado. Intente de nuevo más tarde.");
                registrador.LogError(exception, "Error no controlado en {Metodo} {Ruta}.", httpContext.Request.Method, httpContext.Request.Path);
                break;
        }

        await error.ConvertirAProblema().ExecuteAsync(httpContext);
        return true;
    }
}
