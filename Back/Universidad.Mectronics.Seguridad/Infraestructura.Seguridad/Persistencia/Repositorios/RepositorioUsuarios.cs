using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.ObjetosValor;
using Microsoft.EntityFrameworkCore;
using Universidad.Mectronics.Transversal.Paginacion;

namespace Infraestructura.Seguridad.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IRepositorioUsuarios"/> con Entity Framework. La búsqueda usa
/// <c>(string)(object)propiedad</c> porque EF no traduce <c>.Valor</c> en las propiedades convertidas.
/// </summary>
/// <param name="contexto">Contexto de la petición en curso.</param>
internal sealed class RepositorioUsuarios(ContextoSeguridad contexto) : IRepositorioUsuarios
{
    /// <summary>Carácter de escape para buscar literalmente <c>%</c>, <c>_</c> y <c>[</c> con <c>LIKE</c>.</summary>
    private const string CaracterEscape = "\\";

    /// <summary>
    /// Obtiene una cuenta por su identificador, en seguimiento para poder modificarla.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La cuenta, o <see langword="null"/> si no existe.</returns>
    public Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken cancelacion) =>
        contexto.Usuarios.SingleOrDefaultAsync(usuario => usuario.Id == id, cancelacion);

    /// <summary>
    /// Obtiene una cuenta por su correo, en seguimiento para poder registrar intentos de inicio de sesión.
    /// </summary>
    /// <param name="email">Correo ya normalizado.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La cuenta, o <see langword="null"/> si no existe.</returns>
    public Task<Usuario?> ObtenerPorEmailAsync(CorreoElectronico email, CancellationToken cancelacion) =>
        contexto.Usuarios.SingleOrDefaultAsync(usuario => usuario.Email == email, cancelacion);

    /// <summary>
    /// Indica si otra cuenta ya usa el correo.
    /// </summary>
    /// <param name="email">Correo ya normalizado.</param>
    /// <param name="excluirUsuarioId">Cuenta que se ignora; <see langword="null"/> al crear.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si el correo ya está en uso.</returns>
    public Task<bool> ExisteEmailAsync(CorreoElectronico email, int? excluirUsuarioId, CancellationToken cancelacion) =>
        contexto.Usuarios.AnyAsync(
            usuario => usuario.Email == email && (excluirUsuarioId == null || usuario.Id != excluirUsuarioId),
            cancelacion);

    /// <summary>
    /// Indica si otra cuenta ya usa la identificación.
    /// </summary>
    /// <param name="identificacion">Identificación ya normalizada.</param>
    /// <param name="excluirUsuarioId">Cuenta que se ignora; <see langword="null"/> al crear.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si la identificación ya está en uso.</returns>
    public Task<bool> ExisteIdentificacionAsync(Identificacion identificacion, int? excluirUsuarioId, CancellationToken cancelacion) =>
        contexto.Usuarios.AnyAsync(
            usuario => usuario.Identificacion == identificacion && (excluirUsuarioId == null || usuario.Id != excluirUsuarioId),
            cancelacion);

    /// <summary>
    /// Busca cuentas con los criterios del filtro, ordenadas por nombre, y devuelve la página solicitada.
    /// </summary>
    /// <param name="filtro">Criterios de búsqueda y paginación.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La página de cuentas encontradas.</returns>
    public async Task<Pagina<Usuario>> BuscarAsync(FiltroUsuariosDto filtro, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        IQueryable<Usuario> consulta = contexto.Usuarios.AsNoTracking();

        if (filtro.Texto is not null)
        {
            string patron = $"%{EscaparPatronLike(filtro.Texto)}%";
            consulta = consulta.Where(usuario =>
                EF.Functions.Like((string)(object)usuario.NombreCompleto, patron, CaracterEscape) ||
                EF.Functions.Like((string)(object)usuario.Identificacion, patron, CaracterEscape) ||
                EF.Functions.Like((string)(object)usuario.Email, patron, CaracterEscape));
        }

        if (filtro.RolId is not null)
        {
            consulta = consulta.Where(usuario => usuario.RolId == filtro.RolId);
        }

        if (filtro.Activo is not null)
        {
            consulta = consulta.Where(usuario => usuario.Activo == filtro.Activo);
        }

        int total = await consulta.CountAsync(cancelacion);
        List<Usuario> elementos = await consulta
            .OrderBy(usuario => usuario.NombreCompleto)
            .ThenBy(usuario => usuario.Id)
            .Skip((filtro.NumeroPagina - 1) * filtro.TamanoPagina)
            .Take(filtro.TamanoPagina)
            .ToListAsync(cancelacion);

        return new Pagina<Usuario>(elementos, filtro.NumeroPagina, filtro.TamanoPagina, total);
    }

    /// <summary>
    /// Agrega una cuenta nueva al contexto; se guarda con la unidad de trabajo.
    /// </summary>
    /// <param name="usuario">Cuenta creada con alguna de las fábricas de <see cref="Usuario"/>.</param>
    public void Agregar(Usuario usuario) => contexto.Usuarios.Add(usuario);

    /// <summary>
    /// Escapa los comodines de <c>LIKE</c> para que el texto del usuario se busque literalmente.
    /// </summary>
    /// <param name="texto">Texto escrito por el usuario.</param>
    /// <returns>El texto con los comodines escapados.</returns>
    private static string EscaparPatronLike(string texto) => texto
        .Replace(CaracterEscape, CaracterEscape + CaracterEscape, StringComparison.Ordinal)
        .Replace("%", CaracterEscape + "%", StringComparison.Ordinal)
        .Replace("_", CaracterEscape + "_", StringComparison.Ordinal)
        .Replace("[", CaracterEscape + "[", StringComparison.Ordinal);
}
