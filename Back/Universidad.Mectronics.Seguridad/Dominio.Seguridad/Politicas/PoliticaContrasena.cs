using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.Politicas;

/// <summary>
/// Requisitos de una contraseña: entre 8 y 128 caracteres, con mayúscula, minúscula, número y símbolo, y sin espacios.
/// </summary>
public static class PoliticaContrasena
{
    /// <summary>Cantidad mínima de caracteres de una contraseña.</summary>
    public const int LongitudMinima = 8;

    /// <summary>
    /// Cantidad máxima de caracteres de una contraseña. Evita que se envíen textos enormes
    /// para encarecer a propósito el cálculo del hash.
    /// </summary>
    public const int LongitudMaxima = 128;

    /// <summary>Evalúa la contraseña y devuelve la lista de requisitos que no cumple.</summary>
    /// <param name="contrasena">Contraseña en texto plano.</param>
    /// <returns>Mensajes de los requisitos incumplidos. Una lista vacía significa que la contraseña es
    /// válida.</returns>
    public static IReadOnlyList<string> ObtenerIncumplimientos(string? contrasena)
    {
        if (string.IsNullOrEmpty(contrasena))
        {
            return ["La contraseña es obligatoria."];
        }

        List<string> incumplimientos = [];
        if (contrasena.Length < LongitudMinima)
        {
            incumplimientos.Add($"Debe tener al menos {LongitudMinima} caracteres.");
        }

        if (contrasena.Length > LongitudMaxima)
        {
            incumplimientos.Add($"Debe tener como máximo {LongitudMaxima} caracteres.");
        }

        if (!contrasena.Any(char.IsUpper))
        {
            incumplimientos.Add("Debe incluir al menos una letra mayúscula.");
        }

        if (!contrasena.Any(char.IsLower))
        {
            incumplimientos.Add("Debe incluir al menos una letra minúscula.");
        }

        if (!contrasena.Any(char.IsDigit))
        {
            incumplimientos.Add("Debe incluir al menos un número.");
        }

        if (!contrasena.Any(EsSimbolo))
        {
            incumplimientos.Add("Debe incluir al menos un símbolo.");
        }

        if (contrasena.Any(char.IsWhiteSpace))
        {
            incumplimientos.Add("No puede contener espacios.");
        }

        return incumplimientos;
    }

    /// <summary>
    /// Indica si la contraseña cumple todos los requisitos de la política.
    /// </summary>
    /// <param name="contrasena">Contraseña en texto plano.</param>
    /// <returns><see langword="true"/> si no tiene incumplimientos.</returns>
    public static bool EsValida(string? contrasena) => ObtenerIncumplimientos(contrasena).Count == 0;

    /// <summary>
    /// Valida la contraseña y lanza una excepción si incumple algún requisito.
    /// </summary>
    /// <param name="contrasena">Contraseña en texto plano.</param>
    public static void Validar(string? contrasena)
    {
        IReadOnlyList<string> incumplimientos = ObtenerIncumplimientos(contrasena);
        if (incumplimientos.Count > 0)
        {
            throw new ExcepcionDominio(
                "La contraseña no cumple la política de seguridad. " + string.Join(" ", incumplimientos));
        }
    }

    /// <summary>
    /// Considera símbolo cualquier carácter visible que no sea letra ni número (por ejemplo, <c>#</c>, <c>!</c>, <c>_</c>).
    /// </summary>
    private static bool EsSimbolo(char caracter) =>
        !char.IsLetterOrDigit(caracter) && !char.IsWhiteSpace(caracter) && !char.IsControl(caracter);
}
