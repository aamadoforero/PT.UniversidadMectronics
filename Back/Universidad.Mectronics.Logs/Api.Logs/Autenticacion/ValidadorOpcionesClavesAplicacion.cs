using Microsoft.Extensions.Options;
using Universidad.Mectronics.Transversal.Configuracion;

namespace Api.Logs.Autenticacion;

/// <summary>
/// Valida al arrancar que haya al menos una clave y que todas tengan una longitud segura.
/// </summary>
public sealed class ValidadorOpcionesClavesAplicacion : IValidateOptions<OpcionesClavesAplicacion>
{
    /// <summary>Longitud mínima de cada clave. El script genera 64 caracteres hexadecimales (32 bytes).</summary>
    public const int LongitudMinima = 32;

    /// <summary>
    /// Verifica las claves configuradas.
    /// </summary>
    /// <param name="name">Nombre de la instancia de opciones; no se usa.</param>
    /// <param name="options">Claves leídas de la configuración.</param>
    /// <returns>Éxito, o la lista de problemas encontrados.</returns>
    public ValidateOptionsResult Validate(string? name, OpcionesClavesAplicacion options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Count == 0)
        {
            return ValidateOptionsResult.Fail(
                MensajesConfiguracion.CrearMensajeFaltante($"{OpcionesClavesAplicacion.Seccion}:Servicio-Seguridad"));
        }

        List<string> errores = options
            .Where(entrada => string.IsNullOrWhiteSpace(entrada.Value) || entrada.Value.Length < LongitudMinima)
            .Select(entrada => MensajesConfiguracion.CrearMensajeInvalido(
                $"{OpcionesClavesAplicacion.Seccion}:{entrada.Key}", $"debe tener al menos {LongitudMinima} caracteres."))
            .ToList();

        return errores.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errores);
    }
}
