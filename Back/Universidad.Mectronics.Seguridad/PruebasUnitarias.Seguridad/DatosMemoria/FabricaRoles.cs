using Dominio.Seguridad.Autorizacion;
using Dominio.Seguridad.Entidades;

namespace PruebasUnitarias.Seguridad.DatosMemoria;

/// <summary>
/// Roles de prueba equivalentes a los sembrados en <c>Database/DML.sql</c>.
/// </summary>
public static class FabricaRoles
{
    /// <summary>Identificador del rol SuperAdministrador.</summary>
    public const byte IdSuperAdministrador = 1;

    /// <summary>Identificador del rol Administrador.</summary>
    public const byte IdAdministrador = 2;

    /// <summary>Identificador del rol Profesor.</summary>
    public const byte IdProfesor = 3;

    /// <summary>Identificador del rol Estudiante.</summary>
    public const byte IdEstudiante = 4;

    /// <summary>
    /// Crea el rol SuperAdministrador con todos los permisos, incluido el de acceso total.
    /// </summary>
    /// <returns>El rol.</returns>
    public static Rol CrearSuperAdministrador() => CrearRol(IdSuperAdministrador, "SuperAdministrador", TipoPerfil.Ninguno, CodigosPermiso.Todos);

    /// <summary>
    /// Crea el rol Administrador con todos los permisos de cuentas, sin el de acceso total.
    /// </summary>
    /// <returns>El rol.</returns>
    public static Rol CrearAdministrador() => CrearRol(IdAdministrador, "Administrador", TipoPerfil.Ninguno,
        CodigosPermiso.Todos.Where(codigo => codigo != CodigosPermisoSuperAdministradores.Gestionar));

    /// <summary>
    /// Crea el rol Profesor, sin permisos de administración.
    /// </summary>
    /// <returns>El rol.</returns>
    public static Rol CrearProfesor() => CrearRol(IdProfesor, "Profesor", TipoPerfil.Profesor, ["inscripciones.consultar"]);

    /// <summary>
    /// Crea el rol Estudiante, sin permisos de administración.
    /// </summary>
    /// <returns>El rol.</returns>
    public static Rol CrearEstudiante() => CrearRol(IdEstudiante, "Estudiante", TipoPerfil.Estudiante, ["inscripciones.propias", "inscripciones.consultar"]);

    /// <summary>
    /// Crea los cuatro roles de prueba.
    /// </summary>
    /// <returns>Los roles.</returns>
    public static IReadOnlyList<Rol> CrearTodos() =>
        [CrearSuperAdministrador(), CrearAdministrador(), CrearProfesor(), CrearEstudiante()];

    /// <summary>
    /// Crea un rol con su identificador y sus permisos.
    /// </summary>
    /// <param name="id">Identificador del rol.</param>
    /// <param name="nombre">Nombre del rol.</param>
    /// <param name="tipoPerfil">Datos académicos que exige.</param>
    /// <param name="codigos">Códigos de los permisos.</param>
    /// <returns>El rol.</returns>
    private static Rol CrearRol(byte id, string nombre, TipoPerfil tipoPerfil, IEnumerable<string> codigos)
    {
        Rol rol = AsignadorIdentificador.AsignarId(new Rol(nombre, tipoPerfil), id);
        foreach (string codigo in codigos)
        {
            rol.AsignarPermiso(new Permiso(codigo, $"Permiso {codigo}"));
        }

        return rol;
    }
}
