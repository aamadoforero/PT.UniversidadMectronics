namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Genera contraseñas temporales aleatorias para las cuentas nuevas y los restablecimientos. Infraestructura lo implementa
/// con un generador criptográficamente seguro.
/// </summary>
public interface IGeneradorContrasenaTemporal
{
    /// <summary>
    /// Genera una contraseña aleatoria que cumple la política de contraseñas del dominio.
    /// </summary>
    /// <returns>La contraseña temporal en texto plano, que se muestra una sola vez al administrador.</returns>
    string Generar();
}
