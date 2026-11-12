using System.Diagnostics.CodeAnalysis;
using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.ObjetosValor;

/// <summary>
/// Documento de identidad en mayúsculas, con solo letras, números y guiones.
/// </summary>
public sealed record Identificacion
{
    /// <summary>Longitud máxima de la identificación (columna <c>NVARCHAR(50)</c>).</summary>
    public const int LongitudMaxima = 50;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Identificacion"/>.</summary>
    /// <param name="valor">Identificación normalizada.</param>
    private Identificacion(string valor) => Valor = valor;

    /// <summary>Identificación normalizada, por ejemplo <c>AB-123</c>.</summary>
    public string Valor { get; }

    /// <summary>
    /// Normaliza y valida el texto, y lanza <see cref="ExcepcionDominio"/> si no es una identificación válida.
    /// </summary>
    /// <param name="texto">Identificación tal como la escribió el usuario.</param>
    /// <returns>La identificación normalizada.</returns>
    public static Identificacion Crear(string? texto) =>
        TryCrear(texto, out Identificacion? identificacion)
            ? identificacion
            : throw new ExcepcionDominio(
                $"La identificación es obligatoria, admite letras, números y guiones, y máximo {LongitudMaxima} caracteres.");

    /// <summary>
    /// Intenta normalizar y validar el texto sin lanzar excepciones.
    /// </summary>
    /// <param name="texto">Identificación tal como la escribió el usuario.</param>
    /// <param name="identificacion">La identificación normalizada si es válida; <see langword="null"/> en caso contrario.</param>
    /// <returns><see langword="true"/> si el texto es una identificación válida.</returns>
    public static bool TryCrear(string? texto, [NotNullWhen(true)] out Identificacion? identificacion)
    {
        string normalizada = texto?.Trim().ToUpperInvariant() ?? string.Empty;
        bool esValida = normalizada.Length is > 0 and <= LongitudMaxima &&
                       normalizada.All(caracter => char.IsLetterOrDigit(caracter) || caracter == '-');

        identificacion = esValida ? new Identificacion(normalizada) : null;
        return esValida;
    }

    /// <summary>
    /// Devuelve la identificación normalizada.
    /// </summary>
    /// <returns>El valor de la identificación.</returns>
    public override string ToString() => Valor;
}
