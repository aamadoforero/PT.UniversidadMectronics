namespace Aplicacion.Seguridad.Dtos.Usuarios;

/// <summary>
/// Criterios con que el repositorio busca cuentas de forma paginada. El repositorio excluye siempre las cuentas de
/// superadministración, que están protegidas.
/// </summary>
/// <param name="Texto">Texto a buscar en el nombre, la identificación o el correo; <see langword="null"/> para no filtrar.</param>
/// <param name="RolId">Rol de las cuentas; <see langword="null"/> para todos los roles.</param>
/// <param name="Activo">Estado de las cuentas; <see langword="null"/> para activas e inactivas.</param>
/// <param name="NumeroPagina">Página solicitada, empezando en 1.</param>
/// <param name="TamanoPagina">Cantidad de cuentas por página.</param>
public sealed record FiltroUsuariosDto(
    string? Texto,
    byte? RolId,
    bool? Activo,
    int NumeroPagina,
    int TamanoPagina);
