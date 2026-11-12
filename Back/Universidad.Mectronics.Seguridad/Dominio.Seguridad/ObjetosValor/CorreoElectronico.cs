using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.ObjetosValor;

/// <summary>
/// Correo electrónico de una cuenta, normalizado en minúsculas y sin espacios en los extremos.
/// </summary>
public sealed record CorreoElectronico
{
    /// <summary>Longitud máxima del correo (columna <c>NVARCHAR(150)</c>).</summary>
    public const int LongitudMaxima = 150;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="CorreoElectronico"/>.</summary>
    /// <param name="valor">Correo normalizado.</param>
    private CorreoElectronico(string valor) => Valor = valor;

    /// <summary>Correo normalizado, por ejemplo <c>ana.perez@universidad.edu</c>.</summary>
    public string Valor { get; }

    /// <summary>
    /// Normaliza y valida el texto, y lanza <see cref="ExcepcionDominio"/> si no es un correo válido.
    /// </summary>
    /// <param name="texto">Correo tal como lo escribió el usuario.</param>
    /// <returns>El correo normalizado.</returns>
    public static CorreoElectronico Crear(string? texto) =>
        TryCrear(texto, out CorreoElectronico? correo)
            ? correo
            : throw new ExcepcionDominio($"El correo electrónico no es válido (máximo {LongitudMaxima} caracteres).");

    /// <summary>
    /// Intenta normalizar y validar el texto sin lanzar excepciones. Rechaza formatos con nombre visible,
    /// como <c>Ana &lt;ana@uni.edu&gt;</c>.
    /// </summary>
    /// <param name="texto">Correo tal como lo escribió el usuario.</param>
    /// <param name="correo">El correo normalizado si es válido; <see langword="null"/> en caso contrario.</param>
    /// <returns><see langword="true"/> si el texto es un correo válido.</returns>
    public static bool TryCrear(string? texto, [NotNullWhen(true)] out CorreoElectronico? correo)
    {
        string normalizado = texto?.Trim().ToLowerInvariant() ?? string.Empty;
        bool esValido = normalizado.Length is > 0 and <= LongitudMaxima &&
                       MailAddress.TryCreate(normalizado, out MailAddress? direccion) &&
                       direccion.Address == normalizado;

        correo = esValido ? new CorreoElectronico(normalizado) : null;
        return esValido;
    }

    /// <summary>
    /// Devuelve el correo normalizado.
    /// </summary>
    /// <returns>El valor del correo.</returns>
    public override string ToString() => Valor;
}
