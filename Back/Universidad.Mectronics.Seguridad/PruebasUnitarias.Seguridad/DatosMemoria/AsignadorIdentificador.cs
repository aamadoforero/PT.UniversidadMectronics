namespace PruebasUnitarias.Seguridad.DatosMemoria;

/// <summary>
/// Asigna por reflexión el identificador de una entidad, como lo haría la base de datos al guardarla.
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
