namespace Dominio.Seguridad.Entidades;

/// <summary>Datos académicos que exige un rol al crear una cuenta.</summary>
public enum TipoPerfil : byte
{
    /// <summary>El rol no lleva datos académicos (por ejemplo, Administrador).</summary>
    Ninguno = 0,

    /// <summary>El rol exige programa y ubicación semestral.</summary>
    Estudiante = 1,

    /// <summary>El rol exige categoría de vinculación.</summary>
    Profesor = 2
}
