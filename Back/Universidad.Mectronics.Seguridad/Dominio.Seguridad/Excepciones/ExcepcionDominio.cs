namespace Dominio.Seguridad.Excepciones;

/// <summary>
/// Excepción que se lanza cuando una operación violaría una invariante del dominio. Es una red de seguridad: la capa
/// de Aplicación valida antes los casos esperables y devuelve un resultado de error, así que si esta excepción llega
/// a la API indica un error de programación o una validación omitida, y se responde con HTTP 400.
/// </summary>
/// <param name="mensaje">Descripción legible de la regla incumplida.</param>
public sealed class ExcepcionDominio(string mensaje) : Exception(mensaje);
