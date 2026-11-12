namespace Universidad.Mectronics.Transversal.Auditoria;

/// <summary>
/// Cuerpo de <c>POST /api/logs/eventos</c>. La aplicación de origen se toma de la clave de la cabecera.
/// </summary>
/// <param name="Tipo">Nombre del tipo de evento, uno de <see cref="TiposEvento"/>.</param>
/// <param name="Mensaje">Descripción del evento.</param>
/// <param name="Excepcion">Detalle técnico de la excepción, o <see langword="null"/> si no aplica.</param>
/// <param name="Fecha">Momento en que ocurrió el evento, en UTC.</param>
public sealed record RegistrarEventoDto(string? Tipo, string? Mensaje, string? Excepcion, DateTimeOffset? Fecha);
