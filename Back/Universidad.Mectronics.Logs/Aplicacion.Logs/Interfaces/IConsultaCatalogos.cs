namespace Aplicacion.Logs.Interfaces;

/// <summary>
/// Consulta de los catálogos <c>Logs.TipoEvento</c> y <c>Logs.Aplicacion</c>, que traducen nombres a identificadores.
/// </summary>
public interface IConsultaCatalogos
{
    /// <summary>
    /// Obtiene el identificador de un tipo de evento.
    /// </summary>
    /// <param name="nombre">Nombre del tipo, por ejemplo <c>Advertencia</c>.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El identificador, o <see langword="null"/> si el catálogo no tiene ese tipo.</returns>
    Task<byte?> ObtenerIdTipoEventoAsync(string nombre, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene el identificador de una aplicación.
    /// </summary>
    /// <param name="nombre">Nombre de la aplicación, por ejemplo <c>Servicio-Seguridad</c>.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El identificador, o <see langword="null"/> si la aplicación no está registrada.</returns>
    Task<byte?> ObtenerIdAplicacionAsync(string nombre, CancellationToken cancelacion);
}
