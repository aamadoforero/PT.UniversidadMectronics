namespace Dominio.Academico.Entidades;

/// <summary>
/// Cuenta de usuario, de solo lectura en Académico. Se usa para mostrar nombres y verificar que la cuenta esté activa.
/// </summary>
public sealed class Usuario
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="Usuario"/>.</summary>
    private Usuario()
    {
        NombreCompleto = string.Empty;
    }

    /// <summary>Identificador de la cuenta; es el claim <c>sub</c> del token de acceso.</summary>
    public int Id { get; private set; }

    /// <summary>Nombre completo de la persona.</summary>
    public string NombreCompleto { get; private set; }

    /// <summary>Indica si la cuenta puede iniciar sesión.</summary>
    public bool Activo { get; private set; }
}
