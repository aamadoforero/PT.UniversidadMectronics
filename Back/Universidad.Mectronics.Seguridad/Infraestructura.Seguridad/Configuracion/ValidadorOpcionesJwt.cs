using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Universidad.Mectronics.Transversal.Configuracion;

namespace Infraestructura.Seguridad.Configuracion;

/// <summary>
/// Valida <see cref="OpcionesJwt"/> al arrancar la API (con <c>ValidateOnStart</c>). Si falta una clave o tiene un formato
/// inválido, la API no arranca y el error indica cuál es y cómo corregirla.
/// </summary>
public sealed class ValidadorOpcionesJwt : IValidateOptions<OpcionesJwt>
{
    /// <summary>Tamaño mínimo, en bytes, de las claves HMAC (256 bits, el tamaño de la salida de SHA-256).</summary>
    private const int BytesMinimosHmac = 32;

    /// <summary>Tamaño mínimo, en bits, de la clave RSA.</summary>
    private const int BitsMinimosRsa = 2048;

    /// <summary>
    /// Valida todas las propiedades y reúne los errores encontrados.
    /// </summary>
    /// <param name="name">Nombre de la instancia de opciones (no se usa: hay una sola).</param>
    /// <param name="options">Opciones leídas de la configuración.</param>
    /// <returns>Éxito, o el fallo con todos los mensajes de error.</returns>
    public ValidateOptionsResult Validate(string? name, OpcionesJwt options)
    {
        ArgumentNullException.ThrowIfNull(options);
        List<string> errores = [];

        if (string.IsNullOrWhiteSpace(options.Emisor))
        {
            errores.Add(MensajesConfiguracion.CrearMensajeFaltante($"{OpcionesJwt.Seccion}:{nameof(OpcionesJwt.Emisor)}"));
        }

        if (string.IsNullOrWhiteSpace(options.Audiencia))
        {
            errores.Add(MensajesConfiguracion.CrearMensajeFaltante($"{OpcionesJwt.Seccion}:{nameof(OpcionesJwt.Audiencia)}"));
        }

        if (options.MinutosAcceso is < 1 or > 60)
        {
            errores.Add(MensajesConfiguracion.CrearMensajeInvalido(
                $"{OpcionesJwt.Seccion}:{nameof(OpcionesJwt.MinutosAcceso)}", "debe estar entre 1 y 60."));
        }

        if (options.DiasRenovacion is < 1 or > 30)
        {
            errores.Add(MensajesConfiguracion.CrearMensajeInvalido(
                $"{OpcionesJwt.Seccion}:{nameof(OpcionesJwt.DiasRenovacion)}", "debe estar entre 1 y 30."));
        }

        ValidarClavePrivada(options.ClavePrivadaAcceso, errores);
        ValidarClaveHmac(nameof(OpcionesJwt.ClaveRenovacion), options.ClaveRenovacion, errores);
        ValidarClaveHmac(nameof(OpcionesJwt.ClaveHuella), options.ClaveHuella, errores);

        return errores.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errores);
    }

    /// <summary>
    /// Verifica que la clave privada exista, sea Base64, se pueda importar como PKCS#8 y tenga el tamaño mínimo.
    /// </summary>
    /// <param name="valor">Valor configurado.</param>
    /// <param name="errores">Lista donde se agrega el error, si lo hay.</param>
    private static void ValidarClavePrivada(string valor, List<string> errores)
    {
        string clave = $"{OpcionesJwt.Seccion}:{nameof(OpcionesJwt.ClavePrivadaAcceso)}";
        if (string.IsNullOrWhiteSpace(valor))
        {
            errores.Add(MensajesConfiguracion.CrearMensajeFaltante(clave));
            return;
        }

        try
        {
            using RSA rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(valor), out _);
            if (rsa.KeySize < BitsMinimosRsa)
            {
                errores.Add(MensajesConfiguracion.CrearMensajeInvalido(clave, $"la clave RSA debe tener al menos {BitsMinimosRsa} bits."));
            }
        }
        catch (Exception excepcion) when (excepcion is FormatException or CryptographicException)
        {
            errores.Add(MensajesConfiguracion.CrearMensajeInvalido(
                clave, "se esperaba una clave privada RSA en formato PKCS#8, codificada en Base64 en una sola línea."));
        }
    }

    /// <summary>
    /// Verifica que una clave HMAC exista, sea Base64 y tenga el tamaño mínimo.
    /// </summary>
    /// <param name="propiedad">Nombre de la propiedad dentro de la sección.</param>
    /// <param name="valor">Valor configurado.</param>
    /// <param name="errores">Lista donde se agrega el error, si lo hay.</param>
    private static void ValidarClaveHmac(string propiedad, string valor, List<string> errores)
    {
        string clave = $"{OpcionesJwt.Seccion}:{propiedad}";
        if (string.IsNullOrWhiteSpace(valor))
        {
            errores.Add(MensajesConfiguracion.CrearMensajeFaltante(clave));
            return;
        }

        try
        {
            if (Convert.FromBase64String(valor).Length < BytesMinimosHmac)
            {
                errores.Add(MensajesConfiguracion.CrearMensajeInvalido(clave, $"debe tener al menos {BytesMinimosHmac} bytes."));
            }
        }
        catch (FormatException)
        {
            errores.Add(MensajesConfiguracion.CrearMensajeInvalido(clave, "debe estar codificada en Base64."));
        }
    }
}
