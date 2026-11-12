namespace Dominio.Academico.Entidades;

/// <summary>
/// Perfil de profesor. Es de solo lectura en Académico y se usa para asignarlo a las materias.
/// </summary>
public sealed class Profesor
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="Profesor"/>.</summary>
    private Profesor()
    {
        Usuario = null!;
    }

    /// <summary>Identificador del perfil.</summary>
    public int Id { get; private set; }

    /// <summary>Identificador de la cuenta del profesor.</summary>
    public int UsuarioId { get; private set; }

    /// <summary>Cuenta del profesor, con su nombre.</summary>
    public Usuario Usuario { get; private set; }
}
