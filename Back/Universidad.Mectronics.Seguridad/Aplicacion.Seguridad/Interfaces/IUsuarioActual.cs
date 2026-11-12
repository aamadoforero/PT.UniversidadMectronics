namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Identidad de quien hace la petición, tomada de los claims del token de acceso.
/// </summary>
public interface IUsuarioActual
{
    /// <summary>Indica si la petición viene de una cuenta autenticada.</summary>
    bool EstaAutenticado { get; }

    /// <summary>Identificador de la cuenta autenticada. Leerlo en una petición anónima lanza <see cref="InvalidOperationException"/>.</summary>
    int Id { get; }

    /// <summary>
    /// Indica si la cuenta autenticada tiene un permiso.
    /// </summary>
    /// <param name="codigo">Código del permiso (por ejemplo, <c>usuarios.crear</c>).</param>
    /// <returns><see langword="true"/> si el token de acceso incluye el permiso.</returns>
    bool TienePermiso(string codigo);
}
