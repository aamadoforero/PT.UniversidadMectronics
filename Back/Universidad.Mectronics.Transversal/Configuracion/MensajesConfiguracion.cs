namespace Universidad.Mectronics.Transversal.Configuracion;

/// <summary>Mensajes de error de configuración comunes a los microservicios.</summary>
public static class MensajesConfiguracion
{
    /// <summary>Instrucción común para configurar los secretos.</summary>
    private const string Instruccion =
        "Ejecute scripts/configurar-secretos.sh (Linux/macOS) o scripts/configurar-secretos.ps1 (Windows), " +
        "o consulte la sección de secretos del README.md.";

    /// <summary>
    /// Crea el mensaje para una clave de configuración ausente.
    /// </summary>
    /// <param name="clave">Clave completa, por ejemplo <c>Jwt:ClaveHuella</c>.</param>
    /// <returns>El mensaje con la instrucción para configurarla.</returns>
    public static string CrearMensajeFaltante(string clave) => $"Falta la configuración '{clave}'. {Instruccion}";

    /// <summary>
    /// Crea el mensaje para una clave de configuración con un valor inválido.
    /// </summary>
    /// <param name="clave">Clave completa, por ejemplo <c>Jwt:ClavePrivadaAcceso</c>.</param>
    /// <param name="detalle">Qué se esperaba del valor.</param>
    /// <returns>El mensaje con la instrucción para regenerarla.</returns>
    public static string CrearMensajeInvalido(string clave, string detalle) =>
        $"La configuración '{clave}' no es válida: {detalle} {Instruccion}";
}
