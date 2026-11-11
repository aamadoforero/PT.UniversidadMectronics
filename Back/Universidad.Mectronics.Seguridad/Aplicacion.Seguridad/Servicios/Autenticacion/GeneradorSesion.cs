using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Interfaces;
using Aplicacion.Seguridad.Mapeos;
using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Servicios.Autenticacion;

/// <summary>
/// Arma una sesión completa (token de acceso, token de renovación y datos de la cuenta). La comparten los casos de uso que
/// emiten sesiones: iniciar sesión, renovarla y cambiar la contraseña, que invalida los tokens anteriores.
/// </summary>
/// <param name="servicioTokens">Crea los tokens firmados.</param>
public sealed class GeneradorSesion(IServicioTokens servicioTokens)
{
    /// <summary>
    /// Emite el token de acceso y uno de renovación nuevo, junto con los datos de la cuenta.
    /// </summary>
    /// <param name="usuario">Cuenta autenticada, con su rol y permisos cargados.</param>
    /// <returns>La sesión creada.</returns>
    public SesionCreadaDto CrearSesion(Usuario usuario) => new(
        servicioTokens.CrearTokenAcceso(usuario),
        servicioTokens.CrearTokenRenovacion(usuario),
        usuario.ConvertirAUsuarioSesionDto());
}
