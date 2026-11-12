namespace Aplicacion.Academico.Interfaces;

/// <summary>
/// Acceso de solo lectura a las cuentas de <c>Seguridad.Usuario</c>.
/// </summary>
public interface IRepositorioUsuarios
{
    /// <summary>
    /// Indica si la cuenta existe y sigue activa. La API lo consulta en cada petición.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si la cuenta está activa.</returns>
    Task<bool> EstaActivoAsync(int id, CancellationToken cancelacion);
}
