namespace Universidad.Mectronics.Transversal.Excepciones;

/// <summary>
/// La base de datos rechazó un cambio porque viola una restricción única. Ocurre cuando dos peticiones simultáneas pasan la
/// verificación previa de unicidad; la API la traduce a una respuesta HTTP 409 en lugar de un error 500.
/// </summary>
/// <param name="mensaje">Descripción del conflicto, apta para mostrar al usuario.</param>
/// <param name="excepcionInterna">Excepción original de la base de datos, para el registro técnico.</param>
public sealed class ExcepcionDatoDuplicado(string mensaje, Exception excepcionInterna) : Exception(mensaje, excepcionInterna);
