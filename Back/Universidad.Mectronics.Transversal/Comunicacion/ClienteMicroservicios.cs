using System.Net.Http.Json;

namespace Universidad.Mectronics.Transversal.Comunicacion;

/// <summary>
/// Implementación de <see cref="IClienteMicroservicios"/> sobre el cliente HTTP con la política de resiliencia.
/// </summary>
/// <param name="fabrica">Fábrica de clientes HTTP.</param>
/// <param name="direcciones">URL base de cada microservicio.</param>
internal sealed class ClienteMicroservicios(IHttpClientFactory fabrica, DireccionesMicroservicios direcciones) : IClienteMicroservicios
{
    /// <summary>Nombre del cliente HTTP registrado en la fábrica.</summary>
    public const string NombreCliente = "Microservicios";

    /// <summary>
    /// Envía una petición GET.
    /// </summary>
    /// <typeparam name="T">Tipo del cuerpo de la respuesta.</typeparam>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta con el cuerpo deserializado si fue exitosa.</returns>
    public Task<RespuestaMicroservicio<T>> ObtenerAsync<T>(
        string servicio, string ruta, IReadOnlyDictionary<string, string>? cabeceras = null, CancellationToken cancelacion = default) =>
        EjecutarConContenidoAsync<T>(HttpMethod.Get, servicio, ruta, null, cabeceras, cancelacion);

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
    public Task<RespuestaMicroservicio<T>> EnviarAsync<T>(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default) =>
        EjecutarConContenidoAsync<T>(HttpMethod.Post, servicio, ruta, cuerpo, cabeceras, cancelacion);

    /// <summary>
    /// Envía una petición POST sin leer el cuerpo de la respuesta.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Objeto que se envía como JSON.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta.</returns>
    public Task<RespuestaMicroservicio> EnviarAsync(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default) =>
        EjecutarAsync(HttpMethod.Post, servicio, ruta, cuerpo, cabeceras, cancelacion);

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
    public Task<RespuestaMicroservicio<T>> ActualizarAsync<T>(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default) =>
        EjecutarConContenidoAsync<T>(HttpMethod.Put, servicio, ruta, cuerpo, cabeceras, cancelacion);

    /// <summary>
    /// Envía una petición PUT sin leer el cuerpo de la respuesta.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Objeto que se envía como JSON.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta.</returns>
    public Task<RespuestaMicroservicio> ActualizarAsync(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default) =>
        EjecutarAsync(HttpMethod.Put, servicio, ruta, cuerpo, cabeceras, cancelacion);

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
    public Task<RespuestaMicroservicio<T>> ModificarAsync<T>(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default) =>
        EjecutarConContenidoAsync<T>(HttpMethod.Patch, servicio, ruta, cuerpo, cabeceras, cancelacion);

    /// <summary>
    /// Envía una petición PATCH sin leer el cuerpo de la respuesta.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Objeto que se envía como JSON.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta.</returns>
    public Task<RespuestaMicroservicio> ModificarAsync(
        string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras = null,
        CancellationToken cancelacion = default) =>
        EjecutarAsync(HttpMethod.Patch, servicio, ruta, cuerpo, cabeceras, cancelacion);

    /// <summary>
    /// Envía una petición DELETE.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio en la configuración.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta.</returns>
    public Task<RespuestaMicroservicio> EliminarAsync(
        string servicio, string ruta, IReadOnlyDictionary<string, string>? cabeceras = null, CancellationToken cancelacion = default) =>
        EjecutarAsync(HttpMethod.Delete, servicio, ruta, null, cabeceras, cancelacion);

    /// <summary>
    /// Envía la petición y deserializa el cuerpo de la respuesta exitosa.
    /// </summary>
    /// <typeparam name="T">Tipo del cuerpo de la respuesta.</typeparam>
    /// <param name="metodo">Método HTTP.</param>
    /// <param name="servicio">Nombre del microservicio.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Cuerpo a enviar como JSON, o <see langword="null"/>.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta con su contenido.</returns>
    private async Task<RespuestaMicroservicio<T>> EjecutarConContenidoAsync<T>(
        HttpMethod metodo, string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras,
        CancellationToken cancelacion)
    {
        using HttpResponseMessage respuesta = await EnviarPeticionAsync(metodo, servicio, ruta, cuerpo, cabeceras, cancelacion);

        if (!respuesta.IsSuccessStatusCode)
        {
            string cuerpoError = await respuesta.Content.ReadAsStringAsync(cancelacion);
            return new RespuestaMicroservicio<T>(respuesta.StatusCode, default, cuerpoError);
        }

        T? contenido = respuesta.Content.Headers.ContentLength is 0
            ? default
            : await respuesta.Content.ReadFromJsonAsync<T>(cancelacion);
        return new RespuestaMicroservicio<T>(respuesta.StatusCode, contenido, null);
    }

    /// <summary>
    /// Envía la petición sin leer el cuerpo de la respuesta exitosa.
    /// </summary>
    /// <param name="metodo">Método HTTP.</param>
    /// <param name="servicio">Nombre del microservicio.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Cuerpo a enviar como JSON, o <see langword="null"/>.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta.</returns>
    private async Task<RespuestaMicroservicio> EjecutarAsync(
        HttpMethod metodo, string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras,
        CancellationToken cancelacion)
    {
        using HttpResponseMessage respuesta = await EnviarPeticionAsync(metodo, servicio, ruta, cuerpo, cabeceras, cancelacion);
        string? cuerpoError = respuesta.IsSuccessStatusCode ? null : await respuesta.Content.ReadAsStringAsync(cancelacion);
        return new RespuestaMicroservicio(respuesta.StatusCode, cuerpoError);
    }

    /// <summary>
    /// Arma la petición con la URL del microservicio, el cuerpo JSON y las cabeceras, y la envía.
    /// </summary>
    /// <param name="metodo">Método HTTP.</param>
    /// <param name="servicio">Nombre del microservicio.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <param name="cuerpo">Cuerpo a enviar como JSON, o <see langword="null"/>.</param>
    /// <param name="cabeceras">Cabeceras adicionales, o <see langword="null"/>.</param>
    /// <param name="cancelacion">Token de cancelación.</param>
    /// <returns>La respuesta HTTP.</returns>
    private async Task<HttpResponseMessage> EnviarPeticionAsync(
        HttpMethod metodo, string servicio, string ruta, object? cuerpo, IReadOnlyDictionary<string, string>? cabeceras,
        CancellationToken cancelacion)
    {
        using HttpRequestMessage peticion = new(metodo, direcciones.ConstruirUrl(servicio, ruta));

        if (cuerpo is not null)
        {
            peticion.Content = JsonContent.Create(cuerpo, cuerpo.GetType());
        }

        foreach (KeyValuePair<string, string> cabecera in cabeceras ?? new Dictionary<string, string>())
        {
            peticion.Headers.TryAddWithoutValidation(cabecera.Key, cabecera.Value);
        }

        HttpClient http = fabrica.CreateClient(NombreCliente);
        return await http.SendAsync(peticion, cancelacion);
    }
}
