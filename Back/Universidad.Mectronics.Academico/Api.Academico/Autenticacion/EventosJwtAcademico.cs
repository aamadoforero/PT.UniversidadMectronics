using System.Globalization;
using System.Security.Claims;
using Api.Academico.Errores;
using Aplicacion.Academico.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Academico.Autenticacion;

/// <summary>
/// Eventos de JwtBearer: toma el token de la cookie de acceso que escribe Seguridad, verifica en cada petición que la cuenta
/// siga activa y responde los errores con Problem Details.
/// </summary>
/// <param name="repositorioUsuarios">Consulta del estado de las cuentas.</param>
public sealed class EventosJwtAcademico(IRepositorioUsuarios repositorioUsuarios) : JwtBearerEvents
{
    /// <summary>
    /// Usa la cookie <c>universidad-acceso</c> cuando la petición no trae la cabecera <c>Authorization</c>.
    /// </summary>
    /// <param name="context">Contexto del evento.</param>
    /// <returns>Una tarea completada.</returns>
    public override Task MessageReceived(MessageReceivedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (string.IsNullOrEmpty(context.Request.Headers.Authorization) &&
            context.Request.Cookies.TryGetValue(NombresCookie.Acceso, out string? token))
        {
            context.Token = token;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Rechaza el token si la cuenta ya no existe o fue desactivada, sin esperar a que venza. El rol, los permisos y la
    /// contraseña los valida Seguridad al emitir y renovar el token.
    /// </summary>
    /// <param name="context">Contexto del evento, con el token ya validado.</param>
    /// <returns>Una tarea que termina cuando se verifica la cuenta.</returns>
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ClaimsPrincipal? principal = context.Principal;
        if (principal is null ||
            !int.TryParse(principal.FindFirstValue(NombresClaim.UsuarioId), NumberStyles.None, CultureInfo.InvariantCulture, out int id) ||
            !await repositorioUsuarios.EstaActivoAsync(id, context.HttpContext.RequestAborted))
        {
            context.Fail("La sesión ya no es válida para esta cuenta.");
        }
    }

    /// <summary>
    /// Responde 401 con Problem Details cuando no hay sesión válida.
    /// </summary>
    /// <param name="context">Contexto del evento.</param>
    /// <returns>Una tarea que termina cuando se escribe la respuesta.</returns>
    public override async Task Challenge(JwtBearerChallengeContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.HandleResponse();
        await ErroresApi.CrearNoAutenticado().ConvertirAProblema().ExecuteAsync(context.HttpContext);
    }

    /// <summary>
    /// Responde 403 con Problem Details cuando la sesión no tiene el permiso del endpoint.
    /// </summary>
    /// <param name="context">Contexto del evento.</param>
    /// <returns>Una tarea que termina cuando se escribe la respuesta.</returns>
    public override async Task Forbidden(ForbiddenContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        await ErroresApi.CrearSinPermiso().ConvertirAProblema().ExecuteAsync(context.HttpContext);
    }
}
