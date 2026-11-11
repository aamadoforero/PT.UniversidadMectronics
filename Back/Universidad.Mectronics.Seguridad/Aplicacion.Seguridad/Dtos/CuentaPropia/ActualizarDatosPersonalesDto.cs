namespace Aplicacion.Seguridad.Dtos.CuentaPropia;

/// <summary>
/// Datos que el propio usuario puede modificar desde su perfil. Los datos de identificación y los académicos solo los
/// modifica un usuario con permiso para editar cuentas.
/// </summary>
/// <param name="NombreCompleto">Nombre completo nuevo.</param>
public sealed record ActualizarDatosPersonalesDto(string? NombreCompleto);
