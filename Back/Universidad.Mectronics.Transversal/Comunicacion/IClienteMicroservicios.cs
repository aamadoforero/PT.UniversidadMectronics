namespace Universidad.Mectronics.Transversal.Comunicacion;

/// <summary>
/// Cliente HTTP para llamar a otros microservicios, identificados por su nombre en la sección <c>Microservicios</c> de la
/// configuración. Toda llamada pasa por la política de resiliencia: tiempo máximo, reintentos y circuit breaker
/// independiente por microservicio. Si después de los reintentos no hay respuesta, lanza la excepción del último intento.
/// </summary>
public interface IClienteMicroservicios
{
    /// <summary>
    /// Envía una petición GET.
    /// </summary>
    /// <typeparam name="T">Tipo del cuerpo de la respuesta.</typeparam>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta con el cuerpo deserializado si fue exitosa.</returns>
    Task<RespuestaMicroservicio<T>> ObtenerAsync<T>(
        string servicio, string ruta, IReadOnlyDictionary<string, string>? cabeceras = null, CancellationToken cancelacion = default);

    /// <summary>
    /// Envía una petición POST.
    /// </summary>
    /// <typeparam name="T">Tipo del cuerpo de la respuesta.</typeparam>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Objeto que se envía como JSON.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta con el cuerpo deserializado si fue exitosa.</returns>
    Task<RespuestaMicroservicio<T>> EnviarAsync<T>(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default);

    /// <summary>
    /// Envía una petición POST sin leer el cuerpo de la respuesta.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Objeto que se envía como JSON.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta.</returns>
    Task<RespuestaMicroservicio> EnviarAsync(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default);

    /// <summary>
    /// Envía una petición PUT.
    /// </summary>
    /// <typeparam name="T">Tipo del cuerpo de la respuesta.</typeparam>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Objeto que se envía como JSON.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta con el cuerpo deserializado si fue exitosa.</returns>
    Task<RespuestaMicroservicio<T>> ActualizarAsync<T>(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default);

    /// <summary>
    /// Envía una petición PUT sin leer el cuerpo de la respuesta.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Objeto que se envía como JSON.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta.</returns>
    Task<RespuestaMicroservicio> ActualizarAsync(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default);

    /// <summary>
    /// Envía una petición PATCH.
    /// </summary>
    /// <typeparam name="T">Tipo del cuerpo de la respuesta.</typeparam>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Objeto que se envía como JSON.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta con el cuerpo deserializado si fue exitosa.</returns>
    Task<RespuestaMicroservicio<T>> ModificarAsync<T>(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default);

    /// <summary>
    /// Envía una petición PATCH sin leer el cuerpo de la respuesta.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Objeto que se envía como JSON.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta.</returns>
    Task<RespuestaMicroservicio> ModificarAsync(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default);

    /// <summary>
    /// Envía una petición DELETE.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta.</returns>
    Task<RespuestaMicroservicio> EliminarAsync(
        string servicio, string ruta, IReadOnlyDictionary<string, string>? cabeceras = null, CancellationToken cancelacion = default);
}
