using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>
/// Identidad simulada de quien hace la petición. Por defecto es anónima; <see cref="Autenticar"/> la convierte en la cuenta
/// indicada, con los permisos de su rol.
/// </summary>
public sealed class UsuarioActualFalso : IUsuarioActual
{
    /// <summary>Permisos de la cuenta autenticada.</summary>
    private HashSet<string> _permisos = [];

    /// <summary>Indica si hay una cuenta autenticada.</summary>
    public bool EstaAutenticado { get; private set; }

    /// <summary>Identificador de la cuenta autenticada.</summary>
    public int Id { get; private set; }

    /// <summary>
    /// Simula una sesión de la cuenta indicada.
    /// </summary>
    /// <param name="usuario">Cuenta que hace la petición.</param>
    /// <returns>El mismo doble, para encadenar.</returns>
    public UsuarioActualFalso Autenticar(Usuario usuario)
    {
        EstaAutenticado = true;
        Id = usuario.Id;
        _permisos = usuario.Rol.Permisos.Select(permiso => permiso.Codigo).ToHashSet(StringComparer.Ordinal);
        return this;
    }

    /// <summary>Indica si la cuenta simulada tiene el permiso.</summary>
    /// <param name="codigo">Código del permiso.</param>
    /// <returns><see langword="true"/> si lo tiene.</returns>
    public bool TienePermiso(string codigo) => EstaAutenticado && _permisos.Contains(codigo);
}
