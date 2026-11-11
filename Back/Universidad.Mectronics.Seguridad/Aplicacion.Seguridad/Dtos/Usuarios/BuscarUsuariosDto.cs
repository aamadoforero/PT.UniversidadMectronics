namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Criterios de búsqueda de cuentas que envía el frontend. Todos los filtros son opcionales.
/// </summary>
/// <param name="Texto">Texto a buscar en el nombre, la identificación o el correo.</param>
/// <param name="RolId">Rol de las cuentas.</param>
/// <param name="Activo">Estado de las cuentas.</param>
/// <param name="NumeroPagina">Página solicitada, empezando en 1.</param>
/// <param name="TamanoPagina">Cantidad de cuentas por página, entre 1 y 100.</param>
public sealed record BuscarUsuariosDto(
    string? Texto,
    byte? RolId,
    bool? Activo,
    int NumeroPagina = 1,
    int TamanoPagina = 20);
