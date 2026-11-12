namespace Dominio.Academico.Excepciones;

/// <summary>
/// Excepción que se lanza cuando una operación incumple una regla del dominio. Se responde con HTTP 400.
/// </summary>
/// <param name="mensaje">Descripción legible de la regla incumplida.</param>
public sealed class ExcepcionDominio(string mensaje) : Exception(mensaje);
