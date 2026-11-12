using System.Security.Claims;
using Api.Academico.Errores;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Academico.ContrasenaTemporal;

/// <summary>
/// Responde 403 a las sesiones con contraseña temporal; el cambio de contraseña se hace en Seguridad.
/// </summary>
/// <param name="siguiente">Siguiente componente del pipeline.</param>
public sealed class MiddlewareContrasenaTemporal(RequestDelegate siguiente)
{
    /// <summary>Valor del claim cuando la contraseña es temporal.</summary>
    private const string ValorVerdadero = "true";

    /// <summary>
    /// Responde 403 si la contraseña es temporal y, si no, continúa el pipeline.
    /// </summary>
    /// <param name="contexto">Contexto de la petición.</param>
    /// <returns>Una tarea que termina cuando la petición se atiende.</returns>
    public async Task InvokeAsync(HttpContext contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto);
        if (contexto.User.Identity?.IsAuthenticated == true &&
            contexto.User.FindFirstValue(NombresClaim.DebeCambiarContrasena) == ValorVerdadero)
        {
            await ErroresApi.CrearCambioContrasenaObligatorio().ConvertirAProblema().ExecuteAsync(contexto);
            return;
        }

        await siguiente(contexto);
    }
}
