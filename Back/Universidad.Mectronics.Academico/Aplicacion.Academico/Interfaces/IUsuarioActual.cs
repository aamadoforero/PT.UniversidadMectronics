namespace Aplicacion.Academico.Interfaces;

/// <summary>
/// Cuenta que hace la petición. La implementa la API a partir de los claims del token de acceso emitido por Seguridad.
/// </summary>
public interface IUsuarioActual
{
    /// <summary>Indica si la petición tiene una sesión válida.</summary>
    bool EstaAutenticado { get; }

    /// <summary>Identificador de la cuenta autenticada.</summary>
    int Id { get; }

    /// <summary>
    /// Indica si la cuenta tiene un permiso.
    /// </summary>
    /// <param name="codigo">Código del permiso.</param>
    /// <returns><see langword="true"/> si el token incluye el permiso.</returns>
    bool TienePermiso(string codigo);
}
