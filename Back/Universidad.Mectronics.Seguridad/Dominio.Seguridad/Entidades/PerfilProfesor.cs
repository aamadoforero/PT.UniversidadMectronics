using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.Entidades;

/// <summary>
/// Datos institucionales de una cuenta cuyo rol tiene <see cref="TipoPerfil.Profesor"/>
/// (tabla <c>Academico.Profesor</c>). Pertenece al agregado <see cref="Usuario"/>, así que solo se crea y modifica
/// a través de la cuenta; la categoría es solo una referencia por identificador.
/// </summary>
public sealed class PerfilProfesor
{
    /// <summary>Constructor requerido por Entity Framework para materializar el registro.</summary>
    private PerfilProfesor()
    {
    }

    /// <summary>
    /// Crea el perfil. Solo lo usa <see cref="Usuario.CrearProfesor"/>.
    /// </summary>
    /// <param name="categoriaId">Categoría de vinculación del profesor.</param>
    internal PerfilProfesor(byte categoriaId)
    {
        Actualizar(categoriaId);
    }

    /// <summary>Identificador generado por la base de datos (PK).</summary>
    public int Id { get; private set; }

    /// <summary>Cuenta a la que pertenece el perfil (relación 1 a 1). Lo asigna Entity Framework al guardar.</summary>
    public int UsuarioId { get; private set; }

    /// <summary>Categoría de vinculación: Planta, Cátedra u Ocasional (FK a <c>Academico.CategoriaProfesor</c>).</summary>
    public byte CategoriaId { get; private set; }

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
