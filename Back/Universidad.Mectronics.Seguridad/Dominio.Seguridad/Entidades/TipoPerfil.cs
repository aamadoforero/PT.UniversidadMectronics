namespace Dominio.Seguridad.Entidades;

/// <summary>
/// Datos académicos que exige un rol al crear una cuenta (columna <c>Seguridad.Rol.TipoPerfil</c>). Es un enum y no una tabla
/// porque cada valor tiene comportamiento programado; los valores deben coincidir con el CHECK <c>CK_Rol_TipoPerfil</c>.
/// </summary>
public enum TipoPerfil : byte
{
    /// <summary>El rol no lleva datos académicos (por ejemplo, Administrador).</summary>
    Ninguno = 0,

    /// <summary>El rol exige programa y ubicación semestral (tabla <c>Academico.Estudiante</c>).</summary>
    Estudiante = 1,

    /// <summary>El rol exige categoría de vinculación (tabla <c>Academico.Profesor</c>).</summary>
    Profesor = 2
}
