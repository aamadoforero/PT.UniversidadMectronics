using Aplicacion.Seguridad.Dtos.Catalogos;
using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Mapeos;

/// <summary>
/// Conversiones de la entidad <see cref="Rol"/> a los DTOs que devuelve Aplicación.
/// </summary>
public static class ExtensionesMapeoRol
{
    /// <summary>
    /// Convierte el rol en el elemento del catálogo de roles asignables.
    /// </summary>
    /// <param name="rol">Rol a convertir.</param>
    /// <returns>Los datos del rol.</returns>
    public static RolDto ConvertirARolDto(this Rol rol) => new(rol.Id, rol.Nombre, rol.TipoPerfil);
}
