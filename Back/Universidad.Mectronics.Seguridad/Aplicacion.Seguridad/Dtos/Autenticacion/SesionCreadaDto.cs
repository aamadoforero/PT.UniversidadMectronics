namespace Aplicacion.Seguridad.Dtos.Autenticacion;

/// <summary>
/// Sesión creada al iniciar o renovar sesión. La API entrega los dos tokens en cookies <c>HttpOnly</c> y solo envía
/// en el cuerpo de la respuesta los datos del usuario y los vencimientos.
/// </summary>
/// <param name="TokenAcceso">Token de acceso de vida corta, que validan todos los microservicios.</param>
/// <param name="TokenRenovacion">Token de renovación de vida larga, que solo valida Seguridad.</param>
/// <param name="Usuario">Datos de la cuenta autenticada.</param>
public sealed record SesionCreadaDto(
    TokenEmitidoDto TokenAcceso,
    TokenEmitidoDto TokenRenovacion,
    UsuarioSesionDto Usuario);
