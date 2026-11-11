using Dominio.Seguridad.Entidades;

namespace Dominio.Seguridad.Autorizacion;

/// <summary>
/// Códigos de los permisos para administrar cuentas de usuario. Cada código debe existir en <c>Seguridad.Permiso</c>
/// y forma parte del catálogo <see cref="CodigosPermiso.Todos"/>.
/// </summary>
public static class CodigosPermisoUsuarios
{
    /// <summary>Listar y ver el detalle de las cuentas.</summary>
    public const string Consultar = "usuarios.consultar";

    /// <summary>Crear cuentas nuevas con su perfil académico.</summary>
    public const string Crear = "usuarios.crear";

    /// <summary>Editar los datos de una cuenta y su perfil académico.</summary>
    public const string Editar = "usuarios.editar";

    /// <summary>Activar o desactivar cuentas.</summary>
    public const string CambiarEstado = "usuarios.cambiar-estado";

    /// <summary>Generar una contraseña temporal para una cuenta.</summary>
    public const string RestablecerContrasena = "usuarios.restablecer-contrasena";

    /// <summary>Cambiar el rol de una cuenta (solo entre roles con el mismo <see cref="TipoPerfil"/>).</summary>
    public const string CambiarRol = "usuarios.cambiar-rol";
}
