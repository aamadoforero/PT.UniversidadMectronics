using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Errores;
using Aplicacion.Seguridad.Interfaces;
using Aplicacion.Seguridad.Mapeos;
using Aplicacion.Seguridad.Validadores;
using Dominio.Seguridad.Autorizacion;
using Dominio.Seguridad.Entidades;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Auditoria;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>
/// Implementación de los casos de uso que controlan el acceso de una cuenta: estado y contraseña. Ninguna se permite
/// sobre la propia cuenta.
/// </summary>
/// <param name="autorizador">Reglas de alcance sobre las cuentas y los roles.</param>
/// <param name="repositorioUsuarios">Acceso a las cuentas.</param>
/// <param name="repositorioInscripciones">Cancela las inscripciones del estudiante que se desactiva.</param>
/// <param name="unidadTrabajo">Guarda los cambios.</param>
/// <param name="servicioHash">Calcula el hash de la contraseña temporal.</param>
/// <param name="generadorContrasena">Genera la contraseña temporal.</param>
/// <param name="reloj">Hora actual en UTC.</param>
/// <param name="registroEventos">Auditoría de los cambios de acceso.</param>
/// <param name="validadorCambioEstado">Reglas para cambiar el estado.</param>
public sealed class ServicioAccesoUsuarios(
    AutorizadorUsuarios autorizador,
    IRepositorioUsuarios repositorioUsuarios,
    IRepositorioInscripciones repositorioInscripciones,
    IUnidadTrabajo unidadTrabajo,
    IServicioHashContrasena servicioHash,
    IGeneradorContrasenaTemporal generadorContrasena,
    IReloj reloj,
    IRegistroEventos registroEventos,
    IValidator<CambiarEstadoDto> validadorCambioEstado) : IServicioAccesoUsuarios
{
    /// <summary>
    /// Activa o desactiva una cuenta. Al desactivar un estudiante se cancelan sus inscripciones vigentes en el mismo
    /// guardado.
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
        if (usuario is null)
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

        IReadOnlyList<Inscripcion> inscripcionesCanceladas = [];
        if (activar)
        {
            usuario.Activar(ahora);
        }
        else
        {
            usuario.Desactivar(ahora);
            if (usuario.PerfilEstudiante is not null)
            {
                inscripcionesCanceladas = await repositorioInscripciones.ObtenerVigentesAsync(usuario.PerfilEstudiante.Id, cancelacion);
                foreach (Inscripcion inscripcion in inscripcionesCanceladas)
                {
                    inscripcion.Cancelar(ahora);
                }
            }
        }

        await unidadTrabajo.GuardarCambiosAsync(cancelacion);

        await registroEventos.RegistrarInformacionAsync(
            activar
                ? $"La cuenta {autorizador.IdActor} activó la cuenta {usuario.Id}."
                : $"La cuenta {autorizador.IdActor} desactivó la cuenta {usuario.Id}. Inscripciones canceladas: {inscripcionesCanceladas.Count}.",
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
        if (usuario is null)
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
