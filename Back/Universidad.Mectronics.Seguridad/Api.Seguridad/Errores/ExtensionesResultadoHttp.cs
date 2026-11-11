using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Seguridad.Errores;

/// <summary>
/// Traduce los resultados de la capa de Aplicación a respuestas HTTP. Es el único lugar donde se decide qué código HTTP
/// corresponde a cada <see cref="TipoError"/>, y todos los errores salen con formato Problem Details (RFC 9457).
/// </summary>
public static class ExtensionesResultadoHttp
{
    /// <summary>
    /// Convierte un resultado con datos en 200 con el valor, o en el problema correspondiente.
    /// </summary>
    /// <typeparam name="T">Tipo de los datos del resultado.</typeparam>
    /// <param name="resultado">Resultado del caso de uso.</param>
    /// <returns>La respuesta HTTP.</returns>
    public static IResult ConvertirARespuesta<T>(this Resultado<T> resultado) =>
        resultado.EsExitoso ? TypedResults.Ok(resultado.Valor) : resultado.Error.ConvertirAProblema();

    /// <summary>
    /// Convierte un error en una respuesta Problem Details. El título es el mensaje para el usuario; la extensión
    /// <c>codigo</c> permite al frontend reaccionar sin depender del texto, y <c>errores</c> trae el detalle por campo.
    /// </summary>
    /// <param name="error">Error esperado.</param>
    /// <returns>La respuesta HTTP con el problema.</returns>
    public static IResult ConvertirAProblema(this Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        Dictionary<string, object?> extensiones = new() { ["codigo"] = error.Codigo };
        if (error.Detalles is not null)
        {
            extensiones["errores"] = error.Detalles;
        }

        return TypedResults.Problem(
            title: error.Descripcion,
            statusCode: ObtenerCodigoEstado(error.Tipo),
            extensions: extensiones);
    }

    /// <summary>
    /// Obtiene el código HTTP que corresponde a cada tipo de error.
    /// </summary>
    /// <param name="tipo">Tipo del error.</param>
    /// <returns>El código de estado HTTP.</returns>
    public static int ObtenerCodigoEstado(TipoError tipo) => tipo switch
    {
        TipoError.Validacion => StatusCodes.Status400BadRequest,
        TipoError.NoAutorizado => StatusCodes.Status401Unauthorized,
        TipoError.Prohibido => StatusCodes.Status403Forbidden,
        TipoError.NoEncontrado => StatusCodes.Status404NotFound,
        TipoError.Conflicto => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
