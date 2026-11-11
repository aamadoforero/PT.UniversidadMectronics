using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Errores;

/// <summary>
/// Errores esperados de la consulta de catálogos.
/// </summary>
public static class ErroresCatalogos
{
    /// <summary>
    /// Crea el error para un actor sin ninguno de los permisos que habilitan el catálogo.
    /// </summary>
    /// <returns>El error de falta de permiso.</returns>
    public static Error CrearSinPermiso() => Error.CrearProhibido(
        "Catalogo.SinPermiso",
        "No tiene permiso para consultar este catálogo.");
}
