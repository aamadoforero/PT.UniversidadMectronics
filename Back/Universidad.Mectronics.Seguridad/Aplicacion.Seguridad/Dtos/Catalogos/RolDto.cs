using Dominio.Seguridad.Entidades;

namespace Aplicacion.Seguridad.Dtos.Catalogos;

/// <summary>Rol que se puede asignar a una cuenta, con su tipo de perfil.</summary>
/// <param name="Id">Identificador del rol.</param>
/// <param name="Nombre">Nombre visible del rol.</param>
/// <param name="TipoPerfil">Datos académicos que exige el rol.</param>
public sealed record RolDto(byte Id, string Nombre, TipoPerfil TipoPerfil);
