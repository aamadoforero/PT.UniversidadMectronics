using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Errores;
using Aplicacion.Seguridad.Interfaces;
using Aplicacion.Seguridad.Validadores;
using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.ObjetosValor;
using Dominio.Seguridad.Politicas;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Auditoria;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Autenticacion;

/// <summary>Implementación del inicio y la renovación de sesión.</summary>
/// <param name="repositorioUsuarios">Acceso a las cuentas.</param>
/// <param name="unidadTrabajo">Guarda los intentos fallidos y el reinicio del bloqueo.</param>
/// <param name="servicioHash">Verifica las contraseñas.</param>
/// <param name="servicioTokens">Valida los tokens de renovación y la huella de la contraseña.</param>
/// <param name="reloj">Hora actual en UTC.</param>
/// <param name="registroEventos">Auditoría de inicios de sesión.</param>
/// <param name="politicaBloqueo">Máximo de intentos fallidos y duración del bloqueo.</param>
/// <param name="validador">Reglas de formato de los datos de inicio de sesión.</param>
/// <param name="generadorSesion">Arma la sesión con sus tokens.</param>
public sealed class ServicioAutenticacion(
    IRepositorioUsuarios repositorioUsuarios,
    IUnidadTrabajo unidadTrabajo,
    IServicioHashContrasena servicioHash,
    IServicioTokens servicioTokens,
    IReloj reloj,
    IRegistroEventos registroEventos,
    PoliticaBloqueo politicaBloqueo,
    IValidator<IniciarSesionDto> validador,
    GeneradorSesion generadorSesion) : IServicioAutenticacion
{
    /// <summary>
    /// Verifica las credenciales y, si son correctas, crea una sesión.
    /// </summary>
    /// <param name="datos">Correo y contraseña escritos por el usuario.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La sesión creada, o un error de validación, de credenciales inválidas o de cuenta bloqueada.</returns>
    public async Task<Resultado<SesionCreadaDto>> IniciarSesionAsync(IniciarSesionDto datos, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);

        ValidationResult validacion = await validador.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresAutenticacion.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        CorreoElectronico email = CorreoElectronico.Crear(datos.Email);
        string contrasena = datos.Contrasena!;
        DateTimeOffset ahora = reloj.AhoraUtc;
        Usuario? usuario = await repositorioUsuarios.ObtenerPorEmailAsync(email, cancelacion);

        if (usuario is null || !usuario.Activo)
        {
            servicioHash.Verificar(servicioHash.HashReferencia, contrasena);
            await registroEventos.RegistrarAdvertenciaAsync(
                usuario is null
                    ? "Inicio de sesión rechazado: el correo no está registrado."
                    : $"Inicio de sesión rechazado: la cuenta {usuario.Id} está inactiva.",
                cancelacion);
            return ErroresAutenticacion.CrearCredencialesInvalidas();
        }

        if (usuario.EstaBloqueado(ahora))
        {
            servicioHash.Verificar(servicioHash.HashReferencia, contrasena);
            await registroEventos.RegistrarAdvertenciaAsync(
                $"Inicio de sesión rechazado: la cuenta {usuario.Id} está bloqueada hasta {usuario.BloqueoHasta:O}.",
                cancelacion);
            return ErroresAutenticacion.CrearCuentaBloqueada(politicaBloqueo.DuracionBloqueo);
        }

        if (!servicioHash.Verificar(usuario.Contrasena, contrasena))
        {
            usuario.RegistrarInicioFallido(ahora, politicaBloqueo);
            await unidadTrabajo.GuardarCambiosAsync(cancelacion);
            await registroEventos.RegistrarAdvertenciaAsync(
                usuario.EstaBloqueado(ahora)
                    ? $"La cuenta {usuario.Id} quedó bloqueada hasta {usuario.BloqueoHasta:O} por intentos fallidos."
                    : $"Inicio de sesión fallido para la cuenta {usuario.Id} (intento {usuario.IntentosInicioFallidos}).",
                cancelacion);
            return ErroresAutenticacion.CrearCredencialesInvalidas();
        }

        if (usuario.IntentosInicioFallidos > 0 || usuario.BloqueoHasta.HasValue)
        {
            usuario.RegistrarInicioExitoso();
            await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        }

        await registroEventos.RegistrarInformacionAsync($"Inicio de sesión exitoso de la cuenta {usuario.Id}.", cancelacion);
        return generadorSesion.CrearSesion(usuario);
    }

    /// <summary>
    /// Crea una sesión nueva a partir de un token de renovación válido, con rotación del token de renovación.
    /// </summary>
    /// <param name="datos">Token de renovación recibido en la cookie.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La sesión renovada, o un error de sesión inválida.</returns>
    public async Task<Resultado<SesionCreadaDto>> RenovarSesionAsync(RenovarSesionDto datos, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);
        if (string.IsNullOrWhiteSpace(datos.TokenRenovacion))
        {
            return ErroresAutenticacion.CrearSesionInvalida();
        }

        TokenRenovacionValidoDto? token = await servicioTokens.ValidarTokenRenovacionAsync(datos.TokenRenovacion);
        if (token is null)
        {
            return ErroresAutenticacion.CrearSesionInvalida();
        }

        Usuario? usuario = await repositorioUsuarios.ObtenerPorIdAsync(token.UsuarioId, cancelacion);
        if (usuario is null || !usuario.Activo || !servicioTokens.EsHuellaVigente(usuario, token.HuellaContrasena))
        {
            return ErroresAutenticacion.CrearSesionInvalida();
        }

        return generadorSesion.CrearSesion(usuario);
    }
}
