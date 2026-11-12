using Aplicacion.Seguridad.Interfaces;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>
/// Hash simulado (<c>hash:{contraseña}</c>) que registra las verificaciones realizadas.
/// </summary>
public sealed class ServicioHashFalso : IServicioHashContrasena
{
    /// <summary>Hash de referencia simulado.</summary>
    public string HashReferencia => "hash:referencia";

    /// <summary>Hashes contra los que se verificó, en orden.</summary>
    public List<string> HashesVerificados { get; } = [];

    /// <summary>
    /// Calcula el hash simulado.
    /// </summary>
    /// <param name="contrasena">Contraseña en texto plano.</param>
    /// <returns>El texto <c>hash:{contraseña}</c>.</returns>
    public string CalcularHash(string contrasena) => "hash:" + contrasena;

    /// <summary>
    /// Verifica la contraseña contra el hash simulado y registra la verificación.
    /// </summary>
    /// <param name="hash">Hash guardado.</param>
    /// <param name="contrasena">Contraseña escrita.</param>
    /// <returns><see langword="true"/> si coincide.</returns>
    public bool Verificar(string hash, string contrasena)
    {
        HashesVerificados.Add(hash);
        return hash == CalcularHash(contrasena);
    }
}
