namespace Api.Seguridad.Configuracion;

/// <summary>
/// Configuración del bloqueo de cuentas por intentos fallidos (sección <c>Bloqueo</c>). Con ella se construye la
/// <see cref="Dominio.Seguridad.Politicas.PoliticaBloqueo"/> que usa el inicio de sesión.
/// </summary>
public sealed class OpcionesBloqueo
{
    /// <summary>Nombre de la sección de configuración.</summary>
    public const string Seccion = "Bloqueo";

    /// <summary>Intentos fallidos consecutivos que bloquean la cuenta.</summary>
    public int IntentosMaximos { get; set; } = 5;

    /// <summary>Minutos que dura el bloqueo.</summary>
    public int MinutosBloqueo { get; set; } = 15;
}
