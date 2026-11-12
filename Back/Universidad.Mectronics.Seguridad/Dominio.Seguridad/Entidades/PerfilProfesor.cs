using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.Entidades;

/// <summary>
/// Datos institucionales de una cuenta con <see cref="TipoPerfil.Profesor"/>. Pertenece al agregado
/// <see cref="Usuario"/>.
/// </summary>
public sealed class PerfilProfesor
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="PerfilProfesor"/>.</summary>
    private PerfilProfesor()
    {
    }

    /// <summary>Inicializa una nueva instancia de la clase <see cref="PerfilProfesor"/>.</summary>
    /// <param name="categoriaId">Categoría de vinculación del profesor.</param>
    internal PerfilProfesor(byte categoriaId)
    {
        Actualizar(categoriaId);
    }

    /// <summary>Identificador.</summary>
    public int Id { get; private set; }

    /// <summary>Cuenta a la que pertenece el perfil.</summary>
    public int UsuarioId { get; private set; }

    /// <summary>Categoría de vinculación: Planta, Cátedra u Ocasional.</summary>
    public byte CategoriaId { get; private set; }

    /// <summary>Categoría de vinculación; es <see langword="null"/> en un perfil recién creado.</summary>
    public CategoriaProfesor? Categoria { get; private set; }

    /// <summary>
    /// Cambia la categoría después de validarla.
    /// </summary>
    /// <param name="categoriaId">Categoría de vinculación; debe ser mayor que cero.</param>
    internal void Actualizar(byte categoriaId)
    {
        if (categoriaId == 0)
        {
            throw new ExcepcionDominio(
                "El profesor debe tener una categoría de vinculación.");
        }

        CategoriaId = categoriaId;
    }
}
