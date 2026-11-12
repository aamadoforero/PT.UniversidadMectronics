namespace Universidad.Mectronics.Transversal.Auditoria;

/// <summary>
/// Nombres de los tipos de evento. Coinciden con el catálogo <c>Logs.TipoEvento</c> de <c>Database/DML.sql</c>.
/// </summary>
public static class TiposEvento
{
    /// <summary>Evento normal de la operación (inicio de sesión, creación de una cuenta).</summary>
    public const string Informativo = "Informativo";

    /// <summary>Situación anómala esperada (credenciales incorrectas, cuenta bloqueada).</summary>
    public const string Advertencia = "Advertencia";

    /// <summary>Error inesperado, con el detalle de la excepción.</summary>
    public const string Error = "Error";
}
