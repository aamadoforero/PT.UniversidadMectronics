using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Dtos.CuentaPropia;
using Aplicacion.Seguridad.Errores;
using Aplicacion.Seguridad.Interfaces;
using Aplicacion.Seguridad.Mapeos;
using Aplicacion.Seguridad.Servicios.Autenticacion;
using Aplicacion.Seguridad.Validadores;
using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.ObjetosValor;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.CuentaPropia;

/// <summary>
/// Implementación de los casos de uso sobre la propia cuenta. Identifica la cuenta con <see cref="IUsuarioActual"/> y la
/// vuelve a cargar en cada operación, para rechazar cuentas desactivadas aunque su token de acceso aún no haya vencido.
/// </summary>
/// <param name="usuarioActual">Identidad de quien hace la petición.</param>
/// <param name="repositorioUsuarios">Acceso a las cuentas.</param>
/// <param name="unidadTrabajo">Guarda los cambios de la cuenta.</param>
/// <param name="servicioHash">Verifica la contraseña actual y calcula el hash de la nueva.</param>
/// <param name="reloj">Hora actual en UTC.</param>
/// <param name="registroEventos">Auditoría de los cambios de contraseña.</param>
/// <param name="generadorSesion">Arma la sesión nueva tras cambiar la contraseña.</param>
/// <param name="validadorDatosPersonales">Reglas de formato de los datos personales.</param>
/// <param name="validadorCambioContrasena">Reglas de formato del cambio de contraseña.</param>
public sealed class ServicioCuentaPropia(
    IUsuarioActual usuarioActual,
    IRepositorioUsuarios repositorioUsuarios,
    IUnidadTrabajo unidadTrabajo,
    IServicioHashContrasena servicioHash,
    IReloj reloj,
    IRegistroEventos registroEventos,
    GeneradorSesion generadorSesion,
    IValidator<ActualizarDatosPersonalesDto> validadorDatosPersonales,
    IValidator<CambiarContrasenaDto> validadorCambioContrasena) : IServicioCuentaPropia
{
    /// <summary>
    /// Obtiene el perfil completo de la cuenta autenticada.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El perfil, o un error de sesión inválida.</returns>
    public async Task<Resultado<PerfilDto>> ObtenerPerfilAsync(CancellationToken cancelacion)
    {
        Usuario? usuario = await ObtenerCuentaActivaAsync(cancelacion);
        if (usuario is null)
        {
            return ErroresAutenticacion.CrearSesionInvalida();
        }

        return usuario.ConvertirAPerfilDto();
    }

    /// <summary>
    /// Actualiza el nombre de la cuenta autenticada.
    /// </summary>
    /// <param name="datos">Datos nuevos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El perfil actualizado, o un error de validación o de sesión inválida.</returns>
    public async Task<Resultado<PerfilDto>> ActualizarDatosPersonalesAsync(
        ActualizarDatosPersonalesDto datos,
        CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);

        ValidationResult validacion = await validadorDatosPersonales.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresCuentaPropia.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        Usuario? usuario = await ObtenerCuentaActivaAsync(cancelacion);
        if (usuario is null)
        {
            return ErroresAutenticacion.CrearSesionInvalida();
        }

        usuario.ActualizarDatosPersonales(NombreCompleto.Crear(datos.NombreCompleto), reloj.AhoraUtc);
        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return usuario.ConvertirAPerfilDto();
    }

    /// <summary>
    /// Cambia la contraseña de la cuenta autenticada y entrega una sesión nueva.
    /// </summary>
    /// <param name="datos">Contraseña actual y contraseña nueva.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La sesión nueva, o un error de validación o de sesión inválida.</returns>
    public async Task<Resultado<SesionCreadaDto>> CambiarContrasenaAsync(
        CambiarContrasenaDto datos,
        CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);

        ValidationResult validacion = await validadorCambioContrasena.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresCuentaPropia.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        Usuario? usuario = await ObtenerCuentaActivaAsync(cancelacion);
        if (usuario is null)
        {
            return ErroresAutenticacion.CrearSesionInvalida();
        }

        if (!servicioHash.Verificar(usuario.Contrasena, datos.ContrasenaActual!))
        {
            await registroEventos.RegistrarAdvertenciaAsync(
                $"Cambio de contraseña rechazado para la cuenta {usuario.Id}: la contraseña actual no coincide.",
                cancelacion);
            return ErroresCuentaPropia.CrearContrasenaActualIncorrecta();
        }

        usuario.CambiarContrasena(servicioHash.CalcularHash(datos.ContrasenaNueva!), reloj.AhoraUtc);
        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        await registroEventos.RegistrarInformacionAsync($"La cuenta {usuario.Id} cambió su contraseña.", cancelacion);

        return generadorSesion.CrearSesion(usuario);
    }

    /// <summary>
    /// Carga la cuenta de quien hace la petición, siempre que exista y siga activa.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns>La cuenta activa, o <see langword="null"/> si la petición es anónima, la cuenta no existe o está inactiva.</returns>
    private async Task<Usuario?> ObtenerCuentaActivaAsync(CancellationToken cancelacion)
    {
        if (!usuarioActual.EstaAutenticado)
        {
            return null;
        }

        Usuario? usuario = await repositorioUsuarios.ObtenerPorIdAsync(usuarioActual.Id, cancelacion);
        return usuario is { Activo: true } ? usuario : null;
    }
}
