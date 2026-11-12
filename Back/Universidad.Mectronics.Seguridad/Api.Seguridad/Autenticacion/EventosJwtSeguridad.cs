using System.Globalization;
using System.Security.Claims;
using Api.Seguridad.Errores;
using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Universidad.Mectronics.Transversal.Autenticacion;

namespace Api.Seguridad.Autenticacion;

/// <summary>
/// Eventos de la autenticación JWT: leen el token de la cookie, verifican la cuenta en la base de datos en cada
/// petición y responden 401 y 403 con Problem Details.
/// </summary>
/// <param name="repositorioUsuarios">Acceso a las cuentas.</param>
/// <param name="servicioTokens">Verifica la huella de la contraseña.</param>
public sealed class EventosJwtSeguridad(IRepositorioUsuarios repositorioUsuarios, IServicioTokens servicioTokens) : JwtBearerEvents
{
    /// <summary>
    /// Toma el token de la cookie de acceso. Si la petición trae la cabecera <c>Authorization: Bearer</c> (por ejemplo, desde
    /// una herramienta de pruebas), se respeta esa cabecera.
    /// </summary>
    /// <param name="context">Contexto del mensaje recibido.</param>
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
    /// Con la firma ya verificada, comprueba que la cuenta siga existiendo, activa, con el mismo rol, el mismo estado de
    /// contraseña temporal y la misma contraseña (huella).
    /// </summary>
    /// <param name="context">Contexto del token validado.</param>
    /// <returns>Una tarea que termina cuando la verificación concluye.</returns>
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ClaimsPrincipal? principal = context.Principal;
        if (principal is null ||
            !int.TryParse(principal.FindFirstValue(NombresClaim.UsuarioId), NumberStyles.None, CultureInfo.InvariantCulture, out int id))
        {
            context.Fail("El token no identifica una cuenta.");
            return;
        }

        Usuario? usuario = await repositorioUsuarios.ObtenerPorIdAsync(id, context.HttpContext.RequestAborted);
        bool vigente = usuario is { Activo: true } &&
                       usuario.Rol.Nombre == principal.FindFirstValue(NombresClaim.Rol) &&
                       (usuario.DebeCambiarContrasena ? "true" : "false") == principal.FindFirstValue(NombresClaim.DebeCambiarContrasena) &&
                       servicioTokens.EsHuellaVigente(usuario, principal.FindFirstValue(NombresClaim.HuellaContrasena) ?? string.Empty);
        if (!vigente)
        {
            context.Fail("La sesión ya no es válida para esta cuenta.");
        }
    }

    /// <summary>
    /// Responde 401 con Problem Details cuando falta la sesión o no es válida.
    /// </summary>
    /// <param name="context">Contexto del desafío de autenticación.</param>
    /// <returns>Una tarea que termina cuando la respuesta queda escrita.</returns>
    public override async Task Challenge(JwtBearerChallengeContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.HandleResponse();
        await ErroresApi.CrearNoAutenticado().ConvertirAProblema().ExecuteAsync(context.HttpContext);
    }

    /// <summary>
    /// Responde 403 con Problem Details cuando la sesión no tiene el permiso que exige el endpoint.
    /// </summary>
    /// <param name="context">Contexto del acceso prohibido.</param>
    /// <returns>Una tarea que termina cuando la respuesta queda escrita.</returns>
    public override async Task Forbidden(ForbiddenContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        await ErroresApi.CrearSinPermiso().ConvertirAProblema().ExecuteAsync(context.HttpContext);
    }
}
