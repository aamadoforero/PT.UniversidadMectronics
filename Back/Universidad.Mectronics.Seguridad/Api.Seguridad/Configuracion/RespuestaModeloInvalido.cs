using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.Seguridad.Configuracion;

/// <summary>
/// Respuesta de <c>[ApiController]</c> cuando el cuerpo o los parámetros no se pueden leer (JSON mal formado, tipos
/// incorrectos o cuerpo vacío). Usa el mismo formato Problem Details del resto de la API, con <c>codigo</c> y <c>errores</c>,
/// y mensajes en español en lugar de los mensajes técnicos del deserializador.
/// </summary>
public static class RespuestaModeloInvalido
{
    /// <summary>
    /// Crea la respuesta 400 a partir de los errores de enlace del modelo.
    /// </summary>
    /// <param name="contexto">Contexto de la acción con el estado del modelo.</param>
    /// <returns>La respuesta 400 con el problema.</returns>
    public static IActionResult Crear(ActionContext contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto);
        Dictionary<string, string[]> errores = contexto.ModelState
            .Where(entrada => entrada.Value is { Errors.Count: > 0 })
            .Select(entrada => ObtenerNombreCampo(entrada.Key))
            .Distinct(StringComparer.Ordinal)
            .ToDictionary(campo => campo, _ => new[] { "El valor no tiene un formato válido." }, StringComparer.Ordinal);

        ProblemDetailsFactory fabrica = contexto.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        ProblemDetails problema = fabrica.CreateProblemDetails(
            contexto.HttpContext,
            StatusCodes.Status400BadRequest,
            title: "La solicitud no tiene un formato válido.");
        problema.Extensions["codigo"] = "Solicitud.Invalida";
        problema.Extensions["errores"] = errores;

        return new BadRequestObjectResult(problema) { ContentTypes = { "application/problem+json" } };
    }

    /// <summary>
    /// Limpia la ruta del campo que reporta el deserializador (por ejemplo, <c>$.rolId</c> pasa a <c>rolId</c>).
    /// </summary>
    /// <param name="clave">Clave del estado del modelo.</param>
    /// <returns>El nombre del campo, o <c>cuerpo</c> si el error es del cuerpo completo.</returns>
    private static string ObtenerNombreCampo(string clave)
    {
        string campo = clave.StartsWith("$.", StringComparison.Ordinal) ? clave[2..] : clave;
        return string.IsNullOrEmpty(campo) || campo == "$" || campo == "datos" ? "cuerpo" : campo;
    }
}
