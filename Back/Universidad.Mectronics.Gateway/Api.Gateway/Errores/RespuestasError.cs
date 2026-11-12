namespace Api.Gateway.Errores;

/// <summary>
/// Respuestas de error propias del gateway, con el mismo formato Problem Details de los microservicios.
/// </summary>
public static class RespuestasError
{
    /// <summary>
    /// Crea la respuesta para un microservicio que no acepta conexiones.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio.</param>
    /// <returns>La respuesta 503.</returns>
    public static IResult CrearServicioNoDisponible(string servicio) => CrearProblema(
        StatusCodes.Status503ServiceUnavailable,
        "Gateway.ServicioNoDisponible",
        $"El servicio {servicio} no está disponible en este momento. Intente de nuevo en unos minutos.");

    /// <summary>
    /// Crea la respuesta para un microservicio que no respondió a tiempo.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio.</param>
    /// <returns>La respuesta 504.</returns>
    public static IResult CrearTiempoAgotado(string servicio) => CrearProblema(
        StatusCodes.Status504GatewayTimeout,
        "Gateway.TiempoAgotado",
        $"El servicio {servicio} tardó demasiado en responder. Intente de nuevo.");

    /// <summary>
    /// Crea la respuesta para un fallo de comunicación con el microservicio distinto de los anteriores.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio.</param>
    /// <returns>La respuesta 502.</returns>
    public static IResult CrearErrorComunicacion(string servicio) => CrearProblema(
        StatusCodes.Status502BadGateway,
        "Gateway.ErrorComunicacion",
        $"No fue posible comunicarse con el servicio {servicio}.");

    /// <summary>
    /// Crea la respuesta para un cliente que superó el límite de solicitudes.
    /// </summary>
    /// <returns>La respuesta 429.</returns>
    public static IResult CrearDemasiadasSolicitudes() => CrearProblema(
        StatusCodes.Status429TooManyRequests,
        "Gateway.DemasiadasSolicitudes",
        "Demasiadas solicitudes. Espere un minuto e intente de nuevo.");

    /// <summary>
    /// Crea un Problem Details con el código de error del gateway.
    /// </summary>
    /// <param name="estado">Código HTTP.</param>
    /// <param name="codigo">Código del error para el frontend.</param>
    /// <param name="titulo">Mensaje para el usuario.</param>
    /// <returns>La respuesta con el problema.</returns>
    private static IResult CrearProblema(int estado, string codigo, string titulo) => TypedResults.Problem(
        title: titulo,
        statusCode: estado,
        extensions: new Dictionary<string, object?> { ["codigo"] = codigo });
}
