using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.ObjetosValor;
using PruebasUnitarias.Seguridad.DatosMemoria;
using Universidad.Mectronics.Transversal.Paginacion;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>
/// Repositorio de cuentas en memoria. Asigna identificadores al agregar, como lo haría la base de datos.
/// </summary>
public sealed class RepositorioUsuariosEnMemoria : IRepositorioUsuarios
{
    /// <summary>Siguiente identificador que se asignará.</summary>
    private int _siguienteId = 1000;

    /// <summary>Cuentas guardadas.</summary>
    public List<Usuario> Usuarios { get; } = [];

    /// <summary>
    /// Agrega cuentas existentes al repositorio, para preparar una prueba.
    /// </summary>
    /// <param name="usuarios">Cuentas con su identificador ya asignado.</param>
    /// <returns>El mismo repositorio.</returns>
    public RepositorioUsuariosEnMemoria Sembrar(params Usuario[] usuarios)
    {
        Usuarios.AddRange(usuarios);
        return this;
    }

    /// <summary>Busca por identificador.</summary>
    /// <param name="id">Identificador.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>La cuenta o <see langword="null"/>.</returns>
    public Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken cancelacion) =>
        Task.FromResult(Usuarios.FirstOrDefault(usuario => usuario.Id == id));

    /// <summary>Busca por correo.</summary>
    /// <param name="email">Correo.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>La cuenta o <see langword="null"/>.</returns>
    public Task<Usuario?> ObtenerPorEmailAsync(CorreoElectronico email, CancellationToken cancelacion) =>
        Task.FromResult(Usuarios.FirstOrDefault(usuario => usuario.Email == email));

    /// <summary>Indica si otra cuenta usa el correo.</summary>
    /// <param name="email">Correo.</param>
    /// <param name="excluirUsuarioId">Cuenta que se ignora.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns><see langword="true"/> si está en uso.</returns>
    public Task<bool> ExisteEmailAsync(CorreoElectronico email, int? excluirUsuarioId, CancellationToken cancelacion) =>
        Task.FromResult(Usuarios.Any(usuario => usuario.Email == email && usuario.Id != excluirUsuarioId));

    /// <summary>Indica si otra cuenta usa la identificación.</summary>
    /// <param name="identificacion">Identificación.</param>
    /// <param name="excluirUsuarioId">Cuenta que se ignora.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns><see langword="true"/> si está en uso.</returns>
    public Task<bool> ExisteIdentificacionAsync(Identificacion identificacion, int? excluirUsuarioId, CancellationToken cancelacion) =>
        Task.FromResult(Usuarios.Any(usuario => usuario.Identificacion == identificacion && usuario.Id != excluirUsuarioId));

    /// <summary>Devuelve todas las cuentas sin superadministración en una página.</summary>
    /// <param name="filtro">Criterios de paginación.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>La página.</returns>
    public Task<Pagina<Usuario>> BuscarAsync(FiltroUsuariosDto filtro, CancellationToken cancelacion)
    {
        List<Usuario> visibles = Usuarios.Where(usuario => !usuario.Rol.EsSuperAdministrador).ToList();
        return Task.FromResult(new Pagina<Usuario>(visibles, filtro.NumeroPagina, filtro.TamanoPagina, visibles.Count));
    }

    /// <summary>Agrega una cuenta nueva y le asigna un identificador.</summary>
    /// <param name="usuario">Cuenta nueva.</param>
    public void Agregar(Usuario usuario) => Usuarios.Add(AsignadorIdentificador.AsignarId(usuario, _siguienteId++));
}
