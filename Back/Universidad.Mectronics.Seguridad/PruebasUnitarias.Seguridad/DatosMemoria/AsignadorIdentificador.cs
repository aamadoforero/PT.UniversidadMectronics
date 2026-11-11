namespace PruebasUnitarias.Seguridad.DatosMemoria;

/// <summary>
/// Asigna el identificador a una entidad, como lo haría la base de datos al guardarla. Las entidades lo protegen con un
/// <c>private set</c>, así que solo las pruebas lo asignan, mediante reflexión.
/// </summary>
public static class AsignadorIdentificador
{
    /// <summary>
    /// Asigna el valor de la propiedad <c>Id</c>.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad.</typeparam>
    /// <param name="entidad">Entidad a la que se asigna el identificador.</param>
    /// <param name="id">Identificador, del mismo tipo que la propiedad.</param>
    /// <returns>La misma entidad, para encadenar.</returns>
    public static T AsignarId<T>(T entidad, object id) where T : class
    {
        ArgumentNullException.ThrowIfNull(entidad);
        typeof(T).GetProperty("Id")!.SetValue(entidad, id);
        return entidad;
    }
}
