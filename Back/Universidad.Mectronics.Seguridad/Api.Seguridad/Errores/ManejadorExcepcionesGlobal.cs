using Dominio.Seguridad.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Universidad.Mectronics.Transversal.Auditoria;
using Universidad.Mectronics.Transversal.Excepciones;
using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Seguridad.Errores;

/// <summary>
/// Atrapa las excepciones no controladas, las registra en la auditoría y responde un Problem Details sin detalles
/// internos.
/// </summary>
/// <param name="registroEventos">Auditoría donde se guardan los errores con su detalle técnico.</param>
public sealed class ManejadorExcepcionesGlobal(IRegistroEventos registroEventos) : IExceptionHandler
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

        string origen = $"{httpContext.Request.Method} {httpContext.Request.Path}";
        Error error;
        switch (exception)
        {
            case ExcepcionDominio excepcionDominio:                
                error = new Error("Dominio.ReglaIncumplida", excepcionDominio.Message, TipoError.Validacion);
                await registroEventos.RegistrarAdvertenciaAsync($"Regla de dominio incumplida en {origen}: {excepcionDominio.Message}", cancellationToken);
                break;
            case ExcepcionDatoDuplicado excepcionDuplicado:
                error = Error.CrearConflicto("Datos.Duplicado", excepcionDuplicado.Message);
                break;
            case BadHttpRequestException:
                error = new Error("Solicitud.Invalida", "La solicitud no tiene un formato válido.", TipoError.Validacion);
                break;
            default:
                error = Error.CrearFalla("Servidor.ErrorInesperado", "Ocurrió un error inesperado. Intente de nuevo más tarde.");
                await registroEventos.RegistrarErrorAsync($"Error no controlado en {origen}.", exception, cancellationToken);
                break;
        }

        await error.ConvertirAProblema().ExecuteAsync(httpContext);
        return true;
    }
}
