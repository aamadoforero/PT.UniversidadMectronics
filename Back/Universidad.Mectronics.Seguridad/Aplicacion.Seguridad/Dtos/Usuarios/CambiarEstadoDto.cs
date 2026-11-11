namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Datos para activar o desactivar una cuenta. Es anulable para distinguir un valor omitido de un <see langword="false"/>
/// explícito y no desactivar una cuenta por error.
/// </summary>
/// <param name="Activo"><see langword="true"/> para activar; <see langword="false"/> para desactivar.</param>
public sealed record CambiarEstadoDto(bool? Activo);
