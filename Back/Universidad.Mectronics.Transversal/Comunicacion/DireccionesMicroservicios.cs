namespace Universidad.Mectronics.Transversal.Comunicacion;

/// <summary>
/// URL base de cada microservicio, leída de la sección <c>Microservicios</c> de la configuración.
/// </summary>
internal sealed class DireccionesMicroservicios
{
    /// <summary>Nombre de la sección de configuración.</summary>
    public const string Seccion = "Microservicios";

    /// <summary>URL base por nombre de microservicio.</summary>
    private readonly IReadOnlyDictionary<string, Uri> _direcciones;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="DireccionesMicroservicios"/>.</summary>
    /// <param name="direcciones">URL base por nombre de microservicio.</param>
    public DireccionesMicroservicios(IReadOnlyDictionary<string, Uri> direcciones)
    {
        _direcciones = direcciones;
    }

    /// <summary>
    /// Arma la URL completa de un endpoint.
    /// </summary>
    /// <param name="servicio">Nombre del microservicio.</param>
    /// <param name="ruta">Ruta relativa del endpoint.</param>
    /// <returns>La URL absoluta.</returns>
    public Uri ConstruirUrl(string servicio, string ruta)
    {
        if (!_direcciones.TryGetValue(servicio, out Uri? baseUrl))
        {
            throw new InvalidOperationException($"No hay una URL configurada para el microservicio '{servicio}' en '{Seccion}:{servicio}'.");
        }

        return new Uri(baseUrl, ruta.TrimStart('/'));
    }
}
