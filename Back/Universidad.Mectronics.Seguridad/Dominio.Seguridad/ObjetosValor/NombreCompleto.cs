using System.Diagnostics.CodeAnalysis;
using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.ObjetosValor;

/// <summary>
/// Nombre completo de una persona, sin espacios en los extremos y con los espacios internos repetidos colapsados en uno.
/// Es el único dato visible para los compañeros de clase. Si existe una instancia, el valor ya es válido.
/// </summary>
public sealed record NombreCompleto
{
    /// <summary>Longitud máxima del nombre (columna <c>NVARCHAR(150)</c>).</summary>
    public const int LongitudMaxima = 150;

    /// <summary>
    /// Crea la instancia con un valor ya normalizado y validado. Es privado para obligar a usar <see cref="Crear"/> o <see cref="TryCrear"/>.
    /// </summary>
    /// <param name="valor">Nombre normalizado.</param>
    private NombreCompleto(string valor) => Valor = valor;

    /// <summary>Nombre normalizado, por ejemplo <c>Ana María Pérez</c>.</summary>
    public string Valor { get; }

    /// <summary>
    /// Normaliza y valida el texto, y lanza <see cref="ExcepcionDominio"/> si está vacío o es demasiado largo.
    /// </summary>
    /// <param name="texto">Nombre tal como lo escribió el usuario.</param>
    /// <returns>El nombre normalizado.</returns>
    public static NombreCompleto Crear(string? texto) =>
        TryCrear(texto, out NombreCompleto? nombre)
            ? nombre
            : throw new ExcepcionDominio($"El nombre completo es obligatorio y admite máximo {LongitudMaxima} caracteres.");

    /// <summary>
    /// Intenta normalizar y validar el texto sin lanzar excepciones.
    /// </summary>
    /// <param name="texto">Nombre tal como lo escribió el usuario.</param>
    /// <param name="nombre">El nombre normalizado si es válido; <see langword="null"/> en caso contrario.</param>
    /// <returns><see langword="true"/> si el texto es un nombre válido.</returns>
    public static bool TryCrear(string? texto, [NotNullWhen(true)] out NombreCompleto? nombre)
    {
        string[] partes = (texto ?? string.Empty).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        string normalizado = string.Join(' ', partes);
        bool esValido = normalizado.Length is > 0 and <= LongitudMaxima;

        nombre = esValido ? new NombreCompleto(normalizado) : null;
        return esValido;
    }

    /// <summary>
    /// Devuelve el nombre normalizado.
    /// </summary>
    /// <returns>El valor del nombre.</returns>
    public override string ToString() => Valor;
}
