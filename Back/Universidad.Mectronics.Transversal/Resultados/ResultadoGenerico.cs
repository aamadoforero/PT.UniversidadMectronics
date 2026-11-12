namespace Universidad.Mectronics.Transversal.Resultados;

/// <summary>
/// Resultado de un caso de uso que devuelve datos de tipo <typeparamref name="T"/> cuando es exitoso.
/// </summary>
/// <typeparam name="T">Tipo de los datos que devuelve la operación exitosa.</typeparam>
public sealed class Resultado<T> : Resultado
{
    /// <summary>Datos de la operación; solo tiene valor si fue exitosa.</summary>
    private readonly T? _valor;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Resultado{T}"/>.</summary>
    /// <param name="valor">Datos de la operación exitosa, o el valor por defecto si falló.</param>
    /// <param name="esExitoso">Indica si la operación fue exitosa.</param>
    /// <param name="error">Error del fallo, o <see cref="Error.Ninguno"/> si fue exitosa.</param>
    internal Resultado(T? valor, bool esExitoso, Error error)
        : base(esExitoso, error)
    {
        _valor = valor;
    }

    /// <summary>
    /// Datos de la operación exitosa. Leerlo en un resultado fallido lanza <see cref="InvalidOperationException"/>,
    /// para obligar a revisar <see cref="Resultado.EsExitoso"/> antes de usarlo.
    /// </summary>
    public T Valor => EsExitoso
        ? _valor!
        : throw new InvalidOperationException("No se puede leer el valor de un resultado fallido.");

    /// <summary>
    /// Convierte un valor en un resultado exitoso, para poder escribir <c>return dto;</c>.
    /// </summary>
    /// <param name="valor">Datos que devuelve la operación.</param>
    /// <returns>El resultado exitoso.</returns>
    public static implicit operator Resultado<T>(T valor) => CrearExito(valor);

    /// <summary>
    /// Convierte un error en un resultado fallido, para poder escribir <c>return error;</c>.
    /// </summary>
    /// <param name="error">Error que explica el fallo.</param>
    /// <returns>El resultado fallido.</returns>
    public static implicit operator Resultado<T>(Error error) => CrearFallo<T>(error);
}
