namespace PruebasUnitarias.Academico.DatosMemoria;

/// <summary>
/// Crea entidades de solo lectura (cuentas, programas, profesores y estudiantes) asignando sus propiedades por
/// reflexión.
/// </summary>
public static class FabricaEntidades
{
    /// <summary>
    /// Crea la entidad con su constructor privado sin parámetros.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad.</typeparam>
    /// <returns>La entidad vacía, con sus campos inicializados.</returns>
    public static T Crear<T>() where T : class => (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;

    /// <summary>
    /// Asigna una propiedad de setter privado.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad.</typeparam>
    /// <param name="entidad">Entidad a modificar.</param>
    /// <param name="propiedad">Nombre de la propiedad.</param>
    /// <param name="valor">Valor a asignar.</param>
    /// <returns>La misma entidad, para encadenar.</returns>
    public static T Asignar<T>(this T entidad, string propiedad, object? valor) where T : class
    {
        ArgumentNullException.ThrowIfNull(entidad);
        typeof(T).GetProperty(propiedad)!.SetValue(entidad, valor);
        return entidad;
    }
}
