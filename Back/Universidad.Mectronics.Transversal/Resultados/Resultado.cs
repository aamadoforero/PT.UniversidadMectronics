namespace Universidad.Mectronics.Transversal.Resultados;

/// <summary>
/// Resultado de un caso de uso sin datos: éxito o el <see cref="Error"/> que lo impidió.
/// </summary>
public class Resultado
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="Resultado"/>.</summary>
    /// <param name="esExitoso">Indica si la operación fue exitosa.</param>
    /// <param name="error">Error del fallo, o <see cref="Error.Ninguno"/> si fue exitosa.</param>
    protected Resultado(bool esExitoso, Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        if (esExitoso && error != Error.Ninguno)
        {
            throw new InvalidOperationException("Un resultado exitoso no puede llevar un error.");
        }

        if (!esExitoso && error == Error.Ninguno)
        {
            throw new InvalidOperationException("Un resultado fallido debe indicar su error.");
        }

        EsExitoso = esExitoso;
        Error = error;
    }

    /// <summary>Indica si la operación terminó correctamente.</summary>
    public bool EsExitoso { get; }

    /// <summary>Indica si la operación falló; es lo contrario de <see cref="EsExitoso"/>.</summary>
    public bool EsFallido => !EsExitoso;

    /// <summary>Error del fallo, o <see cref="Error.Ninguno"/> si la operación fue exitosa.</summary>
    public Error Error { get; }

    /// <summary>
    /// Crea un resultado exitoso sin datos.
    /// </summary>
    /// <returns>El resultado exitoso.</returns>
    public static Resultado CrearExito() => new(true, Error.Ninguno);

    /// <summary>
    /// Crea un resultado fallido.
    /// </summary>
    /// <param name="error">Error que explica el fallo.</param>
    /// <returns>El resultado fallido.</returns>
    public static Resultado CrearFallo(Error error) => new(false, error);

    /// <summary>
    /// Crea un resultado exitoso con datos.
    /// </summary>
    /// <typeparam name="T">Tipo de los datos devueltos.</typeparam>
    /// <param name="valor">Datos que devuelve la operación.</param>
    /// <returns>El resultado exitoso con su valor.</returns>
    public static Resultado<T> CrearExito<T>(T valor) => new(valor, true, Error.Ninguno);

    /// <summary>
    /// Crea un resultado fallido para una operación que habría devuelto datos.
    /// </summary>
    /// <typeparam name="T">Tipo de los datos que habría devuelto.</typeparam>
    /// <param name="error">Error que explica el fallo.</param>
    /// <returns>El resultado fallido.</returns>
    public static Resultado<T> CrearFallo<T>(Error error) => new(default, false, error);

    /// <summary>
    /// Convierte un error en un resultado fallido, para poder escribir <c>return error;</c>.
    /// </summary>
    /// <param name="error">Error que explica el fallo.</param>
    /// <returns>El resultado fallido.</returns>
    public static implicit operator Resultado(Error error) => CrearFallo(error);
}
