using Aplicacion.Seguridad.Dtos.Usuarios;
using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.ObjetosValor;
using Universidad.Mectronics.Transversal.Paginacion;

namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Acceso a las cuentas de usuario. Las lecturas devuelven la cuenta con su rol, sus permisos y su perfil académico.
/// </summary>
public interface IRepositorioUsuarios
{
    /// <summary>
    /// Obtiene una cuenta por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La cuenta, o <see langword="null"/> si no existe.</returns>
    Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken cancelacion);

    /// <summary>
    /// Obtiene una cuenta por su correo de inicio de sesión.
    /// </summary>
    /// <param name="email">Correo ya normalizado.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La cuenta, o <see langword="null"/> si no existe.</returns>
    Task<Usuario?> ObtenerPorEmailAsync(CorreoElectronico email, CancellationToken cancelacion);

    /// <summary>
    /// Indica si otra cuenta ya usa el correo.
    /// </summary>
    /// <param name="email">Correo ya normalizado.</param>
    /// <param name="excluirUsuarioId">Cuenta que se ignora en la búsqueda (la que se está editando); <see langword="null"/> al crear.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si el correo ya está en uso.</returns>
    Task<bool> ExisteEmailAsync(CorreoElectronico email, int? excluirUsuarioId, CancellationToken cancelacion);

    /// <summary>
    /// Indica si otra cuenta ya usa la identificación.
    /// </summary>
    /// <param name="identificacion">Identificación ya normalizada.</param>
    /// <param name="excluirUsuarioId">Cuenta que se ignora en la búsqueda (la que se está editando); <see langword="null"/> al crear.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si la identificación ya está en uso.</returns>
    Task<bool> ExisteIdentificacionAsync(Identificacion identificacion, int? excluirUsuarioId, CancellationToken cancelacion);

    /// <summary>
    /// Busca cuentas según los criterios del filtro, ordenadas por nombre, y devuelve solo la página solicitada.
    /// </summary>
    /// <param name="filtro">Criterios de búsqueda y paginación.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La página de cuentas encontradas.</returns>
    Task<Pagina<Usuario>> BuscarAsync(FiltroUsuariosDto filtro, CancellationToken cancelacion);

    /// <summary>
    /// Agrega una cuenta nueva. Se guarda en la base de datos al llamar a <see cref="IUnidadTrabajo.GuardarCambiosAsync"/>.
    /// </summary>
    /// <param name="usuario">Cuenta creada con alguna de las fábricas de <see cref="Usuario"/>.</param>
    void Agregar(Usuario usuario);
}
