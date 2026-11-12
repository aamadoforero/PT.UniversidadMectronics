using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Logs.Errores;

/// <summary>
/// Convierte los resultados de Aplicación en respuestas HTTP con formato Problem Details.
/// </summary>
public static class ExtensionesResultadoHttp
{
    /// <summary>
    /// Convierte un resultado sin datos en 202 (aceptado para procesar después), o en el problema correspondiente.
    /// </summary>
    /// <param name="resultado">Resultado del caso de uso.</param>
    /// <returns>La respuesta HTTP.</returns>
    public static IResult ConvertirARespuestaAceptada(this Resultado resultado)
    {
        ArgumentNullException.ThrowIfNull(resultado);
        return resultado.EsExitoso ? TypedResults.Accepted((string?)null) : resultado.Error.ConvertirAProblema();
    }

    /// <summary>
    /// Convierte un error en una respuesta Problem Details, con las extensiones <c>codigo</c> y, si las hay, <c>errores</c>.
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
        TipoError.DemasiadosIntentos => StatusCodes.Status429TooManyRequests,
        _ => StatusCodes.Status500InternalServerError
    };
}
