using Aplicacion.Logs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infraestructura.Logs.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IConsultaCatalogos"/> con caché en memoria de una hora.
/// </summary>
/// <param name="contexto">Contexto de la petición.</param>
/// <param name="cache">Caché en memoria de la aplicación.</param>
internal sealed class ConsultaCatalogos(ContextoLogs contexto, IMemoryCache cache) : IConsultaCatalogos
{
    /// <summary>Tiempo que un identificador permanece en caché.</summary>
    private static readonly TimeSpan Vigencia = TimeSpan.FromHours(1);

    /// <summary>
    /// Obtiene el identificador de un tipo de evento.
    /// </summary>
    /// <param name="nombre">Nombre del tipo.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El identificador, o <see langword="null"/> si el catálogo no tiene ese tipo.</returns>
    public Task<byte?> ObtenerIdTipoEventoAsync(string nombre, CancellationToken cancelacion) =>
        ObtenerIdAsync($"tipo-evento:{nombre}", contexto.TiposEvento
            .Where(tipo => tipo.Nombre == nombre)
            .Select(tipo => (byte?)tipo.Id), cancelacion);

    /// <summary>
    /// Obtiene el identificador de una aplicación.
    /// </summary>
    /// <param name="nombre">Nombre de la aplicación.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El identificador, o <see langword="null"/> si la aplicación no está registrada.</returns>
    public Task<byte?> ObtenerIdAplicacionAsync(string nombre, CancellationToken cancelacion) =>
        ObtenerIdAsync($"aplicacion:{nombre}", contexto.Aplicaciones
            .Where(aplicacion => aplicacion.Nombre == nombre)
            .Select(aplicacion => (byte?)aplicacion.Id), cancelacion);

    /// <summary>
    /// Devuelve el identificador desde la caché o lo consulta en la base de datos. Los resultados vacíos no se guardan
    /// en caché.
    /// </summary>
    /// <param name="clave">Clave de la caché.</param>
    /// <param name="consulta">Consulta que obtiene el identificador.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El identificador, o <see langword="null"/> si no existe.</returns>
    private async Task<byte?> ObtenerIdAsync(string clave, IQueryable<byte?> consulta, CancellationToken cancelacion)
    {
        if (cache.TryGetValue(clave, out byte guardado))
        {
            return guardado;
        }

        byte? id = await consulta.FirstOrDefaultAsync(cancelacion);
        if (id is not null)
        {
            cache.Set(clave, id.Value, Vigencia);
        }

        return id;
    }
}
