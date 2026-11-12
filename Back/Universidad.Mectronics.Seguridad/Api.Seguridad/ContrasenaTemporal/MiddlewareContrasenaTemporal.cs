using Api.Seguridad.Errores;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Seguridad.ContrasenaTemporal;

/// <summary>
/// Mientras la sesión tenga contraseña temporal, solo permite los endpoints marcados con
/// <see cref="PermiteContrasenaTemporalAttribute"/>.
/// </summary>
/// <param name="siguiente">Siguiente componente del pipeline.</param>
public sealed class MiddlewareContrasenaTemporal(RequestDelegate siguiente)
{
    /// <summary>
    /// Bloquea con 403 las peticiones de una sesión con contraseña temporal a endpoints no permitidos.
    /// </summary>
    /// <param name="contexto">Contexto de la petición, ya autenticada.</param>
    /// <returns>Una tarea que termina cuando la petición se atiende.</returns>
    public async Task InvokeAsync(HttpContext contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto);
        bool contrasenaTemporal = contexto.User.Identity?.IsAuthenticated == true &&
                                  contexto.User.HasClaim(NombresClaim.DebeCambiarContrasena, "true");
        bool permitido = contexto.GetEndpoint()?.Metadata.GetMetadata<PermiteContrasenaTemporalAttribute>() is not null;
        if (contrasenaTemporal && !permitido)
        {
            await ErroresApi.CrearCambioContrasenaObligatorio().ConvertirAProblema().ExecuteAsync(contexto);
            return;
        }

        await siguiente(contexto);
    }
}
