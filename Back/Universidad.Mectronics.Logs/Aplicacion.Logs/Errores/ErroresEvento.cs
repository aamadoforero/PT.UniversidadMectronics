using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Logs.Errores;

/// <summary>
/// Errores esperados del registro de eventos.
/// </summary>
public static class ErroresEvento
{
    /// <summary>
    /// Crea el error para un evento con datos incompletos o con formato inválido.
    /// </summary>
    /// <param name="detalles">Mensajes de error por campo.</param>
    /// <returns>El error de validación.</returns>
    public static Error CrearDatosInvalidos(IReadOnlyDictionary<string, string[]> detalles) => Error.CrearValidacion(
        "Evento.DatosInvalidos",
        "El evento tiene datos inválidos.",
        detalles);

    /// <summary>
    /// Crea el error para una petición sin aplicación autenticada.
    /// </summary>
    /// <returns>El error de tipo no autorizado.</returns>
    public static Error CrearAplicacionNoIdentificada() => Error.CrearNoAutorizado(
        "Evento.AplicacionNoIdentificada",
        "La petición no identifica a la aplicación de origen.");

    /// <summary>
    /// Crea el error para una clave válida cuya aplicación no está en el catálogo <c>Logs.Aplicacion</c>.
    /// </summary>
    /// <param name="nombre">Nombre de la aplicación configurado para la clave.</param>
    /// <returns>El error de falta de permiso.</returns>
    public static Error CrearAplicacionNoRegistrada(string nombre) => Error.CrearProhibido(
        "Evento.AplicacionNoRegistrada",
        $"La aplicación '{nombre}' no está registrada en el catálogo de aplicaciones.");

    /// <summary>
    /// Crea el error para un evento recibido mientras el servicio se detiene y la cola ya no admite eventos.
    /// </summary>
    /// <returns>El error de falla, para que el cliente lo trate como transitorio.</returns>
    public static Error CrearServicioDeteniendose() => Error.CrearFalla(
        "Evento.ServicioDeteniendose",
        "El servicio de Logs se está deteniendo y no puede recibir eventos en este momento.");

    /// <summary>
    /// Crea el error para un tipo de evento que no existe en el catálogo <c>Logs.TipoEvento</c>.
    /// </summary>
    /// <param name="tipo">Nombre del tipo recibido.</param>
    /// <returns>El error de validación del campo <c>Tipo</c>.</returns>
    public static Error CrearTipoNoRegistrado(string tipo) => Error.CrearValidacion(
        "Evento.DatosInvalidos",
        "El evento tiene datos inválidos.",
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["Tipo"] = [$"El tipo de evento '{tipo}' no existe. Use Informativo, Advertencia o Error."]
        });
}
