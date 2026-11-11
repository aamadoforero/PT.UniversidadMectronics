using Aplicacion.Seguridad.Dtos.Usuarios;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>
/// Casos de uso que crean y editan los datos de las cuentas. Exigen los permisos <c>usuarios.crear</c> y
/// <c>usuarios.editar</c>, respectivamente.
/// </summary>
public interface IServicioRegistroUsuarios
{
    /// <summary>
    /// Crea una cuenta con su perfil académico y una contraseña temporal aleatoria.
    /// </summary>
    /// <param name="datos">Datos de la cuenta y, según el rol, sus datos académicos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La cuenta creada con su contraseña temporal, o un error de validación, de conflicto o de falta de permiso.</returns>
    Task<Resultado<UsuarioCreadoDto>> CrearAsync(CrearUsuarioDto datos, CancellationToken cancelacion);

    /// <summary>
    /// Edita los datos de una cuenta y su perfil académico. No cambia el rol.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="datos">Datos nuevos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La cuenta actualizada, o un error de validación, de conflicto, de cuenta inexistente o de falta de permiso.</returns>
    Task<Resultado<UsuarioDetalleDto>> ActualizarAsync(int id, ActualizarUsuarioDto datos, CancellationToken cancelacion);
}
