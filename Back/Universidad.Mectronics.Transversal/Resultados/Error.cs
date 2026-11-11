namespace Universidad.Mectronics.Transversal.Resultados;

/// <summary>
/// Error esperado de un caso de uso. Es un valor (no una excepción): se devuelve dentro de un <see cref="Resultado"/>.
/// El código sigue el formato <c>Entidad.Error</c> (por ejemplo, <c>Usuario.EmailDuplicado</c>) para que el frontend reaccione
/// sin depender del texto; la descripción es apta para mostrar al usuario.
/// </summary>
/// <param name="Codigo">Identificador estable del error, con formato <c>Entidad.Error</c>.</param>
/// <param name="Descripcion">Mensaje legible para el usuario.</param>
/// <param name="Tipo">Categoría del error, que define el código HTTP de la respuesta.</param>
/// <param name="Detalles">Errores por campo (campo y sus mensajes); solo se usa en errores de validación.</param>
public sealed record Error(
    string Codigo,
    string Descripcion,
    TipoError Tipo,
    IReadOnlyDictionary<string, string[]>? Detalles = null)
{
    /// <summary>Valor que representa la ausencia de error; lo lleva todo resultado exitoso.</summary>
    public static Error Ninguno { get; } = new(string.Empty, string.Empty, TipoError.Falla);

    /// <summary>
    /// Crea un error de validación con el detalle de los campos inválidos.
    /// </summary>
    /// <param name="codigo">Identificador del error, con formato <c>Entidad.Error</c>.</param>
    /// <param name="descripcion">Mensaje general para el usuario.</param>
    /// <param name="detalles">Campos inválidos y sus mensajes.</param>
    /// <returns>El error de tipo <see cref="TipoError.Validacion"/>.</returns>
    public static Error CrearValidacion(string codigo, string descripcion, IReadOnlyDictionary<string, string[]> detalles) =>
        new(codigo, descripcion, TipoError.Validacion, detalles);

    /// <summary>
    /// Crea un error de identidad no verificada (credenciales o sesión inválidas).
    /// </summary>
    /// <param name="codigo">Identificador del error, con formato <c>Entidad.Error</c>.</param>
    /// <param name="descripcion">Mensaje para el usuario.</param>
    /// <returns>El error de tipo <see cref="TipoError.NoAutorizado"/>.</returns>
    public static Error CrearNoAutorizado(string codigo, string descripcion) => new(codigo, descripcion, TipoError.NoAutorizado);

    /// <summary>
    /// Crea un error de falta de permiso.
    /// </summary>
    /// <param name="codigo">Identificador del error, con formato <c>Entidad.Error</c>.</param>
    /// <param name="descripcion">Mensaje para el usuario.</param>
    /// <returns>El error de tipo <see cref="TipoError.Prohibido"/>.</returns>
    public static Error CrearProhibido(string codigo, string descripcion) => new(codigo, descripcion, TipoError.Prohibido);

    /// <summary>
    /// Crea un error de recurso inexistente.
    /// </summary>
    /// <param name="codigo">Identificador del error, con formato <c>Entidad.Error</c>.</param>
    /// <param name="descripcion">Mensaje para el usuario.</param>
    /// <returns>El error de tipo <see cref="TipoError.NoEncontrado"/>.</returns>
    public static Error CrearNoEncontrado(string codigo, string descripcion) => new(codigo, descripcion, TipoError.NoEncontrado);

    /// <summary>
    /// Crea un error de conflicto con el estado actual de los datos.
    /// </summary>
    /// <param name="codigo">Identificador del error, con formato <c>Entidad.Error</c>.</param>
    /// <param name="descripcion">Mensaje para el usuario.</param>
    /// <returns>El error de tipo <see cref="TipoError.Conflicto"/>.</returns>
    public static Error CrearConflicto(string codigo, string descripcion) => new(codigo, descripcion, TipoError.Conflicto);

    /// <summary>
    /// Crea un error inesperado de la operación.
    /// </summary>
    /// <param name="codigo">Identificador del error, con formato <c>Entidad.Error</c>.</param>
    /// <param name="descripcion">Mensaje para el usuario.</param>
    /// <returns>El error de tipo <see cref="TipoError.Falla"/>.</returns>
    public static Error CrearFalla(string codigo, string descripcion) => new(codigo, descripcion, TipoError.Falla);
}
