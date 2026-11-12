using Aplicacion.Academico.Interfaces;

namespace Infraestructura.Academico.Tiempo;

/// <summary>
/// Implementación de <see cref="IReloj"/> con la hora del sistema, siempre en UTC (desfase +00:00).
/// </summary>
internal sealed class RelojSistema : IReloj
{
    /// <summary>Fecha y hora actual en UTC.</summary>
    public DateTimeOffset AhoraUtc => DateTimeOffset.UtcNow;
}
