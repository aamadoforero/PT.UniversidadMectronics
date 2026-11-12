namespace Universidad.Mectronics.Transversal.Auditoria;

/// <summary>
/// Rutas del microservicio de Logs. Las comparten su controlador y el cliente que usan los demás microservicios.
/// </summary>
public static class RutasLogs
{
    /// <summary>Ruta para registrar eventos, relativa a la URL base del microservicio.</summary>
    public const string Eventos = "api/logs/eventos";
}
