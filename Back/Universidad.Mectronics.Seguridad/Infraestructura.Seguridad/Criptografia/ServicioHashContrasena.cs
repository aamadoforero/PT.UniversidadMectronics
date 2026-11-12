using Aplicacion.Seguridad.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infraestructura.Seguridad.Criptografia;

/// <summary>
/// Implementación de <see cref="IServicioHashContrasena"/> con el <see cref="PasswordHasher{TUser}"/> de ASP.NET Core
/// (PBKDF2).
/// </summary>
internal sealed class ServicioHashContrasena : IServicioHashContrasena
{
    /// <summary>
    /// Usuario vacío que exige la firma de <see cref="PasswordHasher{TUser}"/>.
    /// </summary>
    private static readonly object UsuarioSinDatos = new();

    /// <summary>Implementación de ASP.NET Core que calcula y verifica los hashes.</summary>
    private readonly PasswordHasher<object> _hasher = new();

    /// <summary>Inicializa una nueva instancia de la clase <see cref="ServicioHashContrasena"/>.</summary>
    public ServicioHashContrasena()
    {
        HashReferencia = _hasher.HashPassword(UsuarioSinDatos, Guid.NewGuid().ToString("N"));
    }

    /// <summary>Hash de una contraseña aleatoria; se verifica contra él cuando el correo no existe, para igualar el tiempo de respuesta.</summary>
    public string HashReferencia { get; }

    /// <summary>
    /// Calcula el hash de una contraseña con una sal aleatoria nueva.
    /// </summary>
    /// <param name="contrasena">Contraseña en texto plano.</param>
    /// <returns>El hash en formato V3 de ASP.NET Core, codificado en Base64.</returns>
    public string CalcularHash(string contrasena) => _hasher.HashPassword(UsuarioSinDatos, contrasena);

    /// <summary>
    /// Verifica si la contraseña corresponde al hash. Un hash con formato inválido se trata como contraseña incorrecta.
    /// </summary>
    /// <param name="hash">Hash guardado.</param>
    /// <param name="contrasena">Contraseña escrita por el usuario.</param>
    /// <returns><see langword="true"/> si la contraseña es correcta.</returns>
    public bool Verificar(string hash, string contrasena)
    {
        try
        {
            return _hasher.VerifyHashedPassword(UsuarioSinDatos, hash, contrasena) != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
