using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Errores;
using Aplicacion.Seguridad.Interfaces;
using Aplicacion.Seguridad.Mapeos;
using Aplicacion.Seguridad.Validadores;
using Dominio.Seguridad.Autorizacion;
using Dominio.Seguridad.Entidades;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>
/// Implementación de los casos de uso que controlan el acceso de una cuenta: rol, estado y contraseña. La cuenta del
/// superadministrador está protegida y su rol no se asigna, así que estas operaciones nunca la afectan.
/// </summary>
/// <param name="autorizador">Reglas de alcance sobre las cuentas y los roles.</param>
/// <param name="repositorioUsuarios">Acceso a las cuentas.</param>
/// <param name="repositorioRoles">Acceso a los roles.</param>
/// <param name="unidadTrabajo">Guarda los cambios.</param>
/// <param name="servicioHash">Calcula el hash de la contraseña temporal.</param>
/// <param name="generadorContrasena">Genera la contraseña temporal.</param>
/// <param name="reloj">Hora actual en UTC.</param>
/// <param name="registroEventos">Auditoría de los cambios de acceso.</param>
/// <param name="validadorCambioRol">Reglas para cambiar el rol.</param>
/// <param name="validadorCambioEstado">Reglas para cambiar el estado.</param>
public sealed class ServicioAccesoUsuarios(
    AutorizadorUsuarios autorizador,
    IRepositorioUsuarios repositorioUsuarios,
    IRepositorioRoles repositorioRoles,
    IUnidadTrabajo unidadTrabajo,
    IServicioHashContrasena servicioHash,
    IGeneradorContrasenaTemporal generadorContrasena,
    IReloj reloj,
    IRegistroEventos registroEventos,
    IValidator<CambiarRolDto> validadorCambioRol,
    IValidator<CambiarEstadoDto> validadorCambioEstado) : IServicioAccesoUsuarios
{
    /// <summary>
    /// Cambia el rol de una cuenta hacia otro con el mismo tipo de perfil.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="datos">Rol nuevo.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La cuenta actualizada, o un error.</returns>
    public async Task<Resultado<UsuarioDetalleDto>> CambiarRolAsync(
        int id,
        CambiarRolDto datos,
        CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);
        if (!autorizador.TienePermiso(CodigosPermisoUsuarios.CambiarRol))
        {
            return ErroresUsuario.CrearSinPermiso();
        }

        ValidationResult validacion = await validadorCambioRol.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresUsuario.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        Usuario? usuario = await repositorioUsuarios.ObtenerPorIdAsync(id, cancelacion);
        if (usuario is null || !autorizador.PuedeGestionar(usuario))
        {
            return ErroresUsuario.CrearNoEncontrado(id);
        }

        if (autorizador.EsCuentaPropia(usuario))
        {
            return ErroresUsuario.CrearOperacionSobreCuentaPropia("No puede cambiar el rol de su propia cuenta.");
        }

        Rol? nuevoRol = await repositorioRoles.ObtenerPorIdAsync(datos.RolId!.Value, cancelacion);
        if (nuevoRol is null)
        {
            return ErroresUsuario.CrearRolNoEncontrado();
        }

        if (!autorizador.PuedeAsignar(nuevoRol))
        {
            return ErroresUsuario.CrearRolNoAsignable();
        }

        DateTimeOffset ahora = reloj.AhoraUtc;
        if (nuevoRol.Id == usuario.RolId)
        {
            return usuario.ConvertirAUsuarioDetalleDto(ahora);
        }

        if (nuevoRol.TipoPerfil != usuario.Rol.TipoPerfil)
        {
            return ErroresUsuario.CrearCambioRolNoPermitido(usuario.Rol.TipoPerfil, nuevoRol.TipoPerfil);
        }

        string rolAnterior = usuario.Rol.Nombre;
        usuario.CambiarRol(nuevoRol, ahora);
        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        await registroEventos.RegistrarInformacionAsync(
            $"La cuenta {autorizador.IdActor} cambió el rol de la cuenta {usuario.Id} de {rolAnterior} a {nuevoRol.Nombre}.",
            cancelacion);

        return usuario.ConvertirAUsuarioDetalleDto(ahora);
    }

    /// <summary>
    /// Activa o desactiva una cuenta.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="datos">Estado nuevo.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La cuenta actualizada, o un error.</returns>
    public async Task<Resultado<UsuarioDetalleDto>> CambiarEstadoAsync(
        int id,
        CambiarEstadoDto datos,
        CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);
        if (!autorizador.TienePermiso(CodigosPermisoUsuarios.CambiarEstado))
        {
            return ErroresUsuario.CrearSinPermiso();
        }

        ValidationResult validacion = await validadorCambioEstado.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresUsuario.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        Usuario? usuario = await repositorioUsuarios.ObtenerPorIdAsync(id, cancelacion);
        if (usuario is null || !autorizador.PuedeGestionar(usuario))
        {
            return ErroresUsuario.CrearNoEncontrado(id);
        }

        bool activar = datos.Activo!.Value;
        if (!activar && autorizador.EsCuentaPropia(usuario))
        {
            return ErroresUsuario.CrearOperacionSobreCuentaPropia("No puede desactivar su propia cuenta.");
        }

        DateTimeOffset ahora = reloj.AhoraUtc;
        if (usuario.Activo == activar)
        {
            return usuario.ConvertirAUsuarioDetalleDto(ahora);
        }

        if (activar)
        {
            usuario.Activar(ahora);
        }
        else
        {
            usuario.Desactivar(ahora);
        }

        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        await registroEventos.RegistrarInformacionAsync(
            $"La cuenta {autorizador.IdActor} {(activar ? "activó" : "desactivó")} la cuenta {usuario.Id}.",
            cancelacion);

        return usuario.ConvertirAUsuarioDetalleDto(ahora);
    }

    /// <summary>
    /// Genera una contraseña temporal para una cuenta.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La contraseña temporal, o un error.</returns>
    public async Task<Resultado<ContrasenaRestablecidaDto>> RestablecerContrasenaAsync(int id, CancellationToken cancelacion)
    {
        if (!autorizador.TienePermiso(CodigosPermisoUsuarios.RestablecerContrasena))
        {
            return ErroresUsuario.CrearSinPermiso();
        }

        Usuario? usuario = await repositorioUsuarios.ObtenerPorIdAsync(id, cancelacion);
        if (usuario is null || !autorizador.PuedeGestionar(usuario))
        {
            return ErroresUsuario.CrearNoEncontrado(id);
        }

        if (autorizador.EsCuentaPropia(usuario))
        {
            return ErroresUsuario.CrearOperacionSobreCuentaPropia(
                "Para cambiar su propia contraseña use la opción de cambio de contraseña de su perfil.");
        }

        string contrasenaTemporal = generadorContrasena.Generar();
        usuario.AsignarContrasenaTemporal(servicioHash.CalcularHash(contrasenaTemporal), reloj.AhoraUtc);
        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        await registroEventos.RegistrarInformacionAsync(
            $"La cuenta {autorizador.IdActor} restableció la contraseña de la cuenta {usuario.Id}.", cancelacion);

        return new ContrasenaRestablecidaDto(usuario.Id, contrasenaTemporal);
    }
}
