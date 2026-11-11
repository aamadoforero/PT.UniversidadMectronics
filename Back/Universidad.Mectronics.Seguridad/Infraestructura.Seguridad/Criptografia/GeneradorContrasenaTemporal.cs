using System.Security.Cryptography;
using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Politicas;

namespace Infraestructura.Seguridad.Criptografia;

/// <summary>
/// Implementación de <see cref="IGeneradorContrasenaTemporal"/> con <see cref="RandomNumberGenerator"/>, que es
/// criptográficamente seguro. Garantiza al menos un carácter de cada tipo que exige <see cref="PoliticaContrasena"/> y omite
/// los que se confunden al leerlos (I, l, O, 0, 1).
/// </summary>
internal sealed class GeneradorContrasenaTemporal : IGeneradorContrasenaTemporal
{
    /// <summary>Longitud de la contraseña temporal.</summary>
    private const int Longitud = 14;

    /// <summary>Mayúsculas permitidas (sin I ni O).</summary>
    private const string Mayusculas = "ABCDEFGHJKLMNPQRSTUVWXYZ";

    /// <summary>Minúsculas permitidas (sin l ni o).</summary>
    private const string Minusculas = "abcdefghijkmnpqrstuvwxyz";

    /// <summary>Números permitidos (sin 0 ni 1).</summary>
    private const string Numeros = "23456789";

    /// <summary>Símbolos permitidos, fáciles de escribir en cualquier teclado.</summary>
    private const string Simbolos = "#$%&*+-=?@";

    /// <summary>
    /// Genera una contraseña aleatoria que cumple la política de contraseñas.
    /// </summary>
    /// <returns>La contraseña temporal.</returns>
    public string Generar()
    {
        string todos = Mayusculas + Minusculas + Numeros + Simbolos;
        char[] caracteres = new char[Longitud];

        caracteres[0] = Mayusculas[RandomNumberGenerator.GetInt32(Mayusculas.Length)];
        caracteres[1] = Minusculas[RandomNumberGenerator.GetInt32(Minusculas.Length)];
        caracteres[2] = Numeros[RandomNumberGenerator.GetInt32(Numeros.Length)];
        caracteres[3] = Simbolos[RandomNumberGenerator.GetInt32(Simbolos.Length)];
        for (int posicion = 4; posicion < Longitud; posicion++)
        {
            caracteres[posicion] = todos[RandomNumberGenerator.GetInt32(todos.Length)];
        }

        RandomNumberGenerator.Shuffle(caracteres.AsSpan());
        string contrasena = new(caracteres);

        PoliticaContrasena.Validar(contrasena);
        return contrasena;
    }
}
