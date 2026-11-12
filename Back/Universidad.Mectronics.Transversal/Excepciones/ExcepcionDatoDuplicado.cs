namespace Universidad.Mectronics.Transversal.Excepciones;

/// <summary>
/// La base de datos rechazó un cambio por una restricción única. Se responde con HTTP 409.
/// </summary>
/// <param name="mensaje">Descripción del conflicto, apta para mostrar al usuario.</param>
/// <param name="excepcionInterna">Excepción original de la base de datos, para el registro técnico.</param>
public sealed class ExcepcionDatoDuplicado(string mensaje, Exception excepcionInterna) : Exception(mensaje, excepcionInterna);
