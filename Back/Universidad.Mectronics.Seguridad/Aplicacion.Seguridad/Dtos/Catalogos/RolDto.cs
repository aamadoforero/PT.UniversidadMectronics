using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Dtos.Catalogos;

/// <summary>
/// Rol que se puede asignar a una cuenta. Incluye el tipo de perfil para que el formulario muestre los campos académicos
/// que corresponden (programa y semestre, o categoría).
/// </summary>
/// <param name="Id">Identificador del rol.</param>
/// <param name="Nombre">Nombre visible del rol.</param>
/// <param name="TipoPerfil">Datos académicos que exige el rol.</param>
public sealed record RolDto(byte Id, string Nombre, TipoPerfil TipoPerfil);
