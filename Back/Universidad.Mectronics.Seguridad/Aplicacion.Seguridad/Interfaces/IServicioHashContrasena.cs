namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Calcula y verifica hashes de contraseña. Aplicación no conoce el algoritmo; Infraestructura lo implementa con el
/// <c>PasswordHasher</c> de ASP.NET Core (PBKDF2-HMAC-SHA512).
/// </summary>
public interface IServicioHashContrasena
{
    /// <summary>
    /// Hash de una contraseña que nadie conoce. Se verifica cuando el correo no existe para que la respuesta tarde lo
    /// mismo.
    /// </summary>
    string HashReferencia { get; }

    /// <summary>
    /// Calcula el hash de una contraseña, con una sal aleatoria nueva.
    /// </summary>
    /// <param name="contrasena">Contraseña en texto plano.</param>
    /// <returns>El hash, listo para guardarse.</returns>
    string CalcularHash(string contrasena);

    /// <summary>
    /// Verifica si una contraseña corresponde a un hash.
    /// </summary>
    /// <param name="hash">Hash guardado de la cuenta.</param>
    /// <param name="contrasena">Contraseña en texto plano que escribió el usuario.</param>
    /// <returns><see langword="true"/> si la contraseña es correcta.</returns>
    bool Verificar(string hash, string contrasena);
}
