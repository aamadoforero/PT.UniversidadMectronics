using Dominio.Seguridad.Autorizacion;
using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.Entidades;

/// <summary>
/// Rol de autorización. Sus permisos definen lo que puede hacer y su <see cref="TipoPerfil"/> los datos académicos que
/// exige.
/// </summary>
public sealed class Rol
{
    /// <summary>Longitud máxima del nombre (columna <c>NVARCHAR(50)</c>).</summary>
    public const int LongitudMaximaNombre = 50;

    /// <summary>Permisos asignados al rol.</summary>
    private readonly List<Permiso> _permisos = [];

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Rol"/>.</summary>
    private Rol()
    {
        Nombre = string.Empty;
    }

    /// <summary>Inicializa una nueva instancia de la clase <see cref="Rol"/>.</summary>
    /// <param name="nombre">Nombre único del rol. Se quitan los espacios de los extremos.</param>
    /// <param name="tipoPerfil">Datos académicos que exigirán sus cuentas. Por defecto, ninguno.</param>
    public Rol(string nombre, TipoPerfil tipoPerfil = TipoPerfil.Ninguno)
    {
        string normalizado = nombre?.Trim() ?? string.Empty;
        if (normalizado.Length == 0 || normalizado.Length > LongitudMaximaNombre)
        {
            throw new ExcepcionDominio(
                $"El nombre del rol es obligatorio (máximo {LongitudMaximaNombre} caracteres).");
        }

        if (!Enum.IsDefined(tipoPerfil))
        {
            throw new ExcepcionDominio(
                $"El tipo de perfil '{(byte)tipoPerfil}' no es válido.");
        }

        Nombre = normalizado;
        TipoPerfil = tipoPerfil;
    }

    /// <summary>Identificador.</summary>
    public byte Id { get; private set; }

    /// <summary>Nombre único del rol. Es informativo: ninguna regla depende de él.</summary>
    public string Nombre { get; private set; }

    /// <summary>
    /// Datos académicos que exigen las cuentas con este rol. No se puede cambiar.
    /// </summary>
    public TipoPerfil TipoPerfil { get; private set; }

    /// <summary>Permisos que habilita el rol. Es de solo lectura; se modifica con <see cref="AsignarPermiso"/> y <see cref="RevocarPermiso"/>.</summary>
    public IReadOnlyCollection<Permiso> Permisos => _permisos;

    /// <summary>Indica si las cuentas con este rol llevan datos académicos (estudiante o profesor).</summary>
    public bool TienePerfilAcademico => TipoPerfil != TipoPerfil.Ninguno;

    /// <summary>
    /// Indica si el rol tiene el permiso indicado.
    /// </summary>
    /// <param name="codigo">Código del permiso (por ejemplo, <see cref="CodigosPermisoUsuarios.Crear"/>).</param>
    /// <returns><see langword="true"/> si el permiso está asignado al rol.</returns>
    public bool TienePermiso(string codigo) =>
        _permisos.Any(permiso => permiso.Codigo == codigo);

    /// <summary>
    /// Asigna un permiso al rol. Si ya lo tiene, no hace nada.
    /// </summary>
    /// <param name="permiso">Permiso a asignar.</param>
    public void AsignarPermiso(Permiso permiso)
    {
        ArgumentNullException.ThrowIfNull(permiso);
        if (!TienePermiso(permiso.Codigo))
        {
            _permisos.Add(permiso);
        }
    }

    /// <summary>
    /// Quita un permiso del rol. Si no lo tiene, no hace nada.
    /// </summary>
    /// <param name="codigo">Código del permiso a quitar.</param>
    public void RevocarPermiso(string codigo) =>
        _permisos.RemoveAll(permiso => permiso.Codigo == codigo);
}
