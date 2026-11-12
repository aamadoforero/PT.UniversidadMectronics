using Aplicacion.Academico.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Academico.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IRepositorioUsuarios"/> con Entity Framework.
/// </summary>
/// <param name="contexto">Contexto de la petición.</param>
internal sealed class RepositorioUsuarios(ContextoAcademico contexto) : IRepositorioUsuarios
{
    /// <summary>
    /// Indica si la cuenta existe y sigue activa. Es una búsqueda por clave primaria.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la consulta si la petición se abandona.</param>
    /// <returns><see langword="true"/> si la cuenta está activa.</returns>
    public Task<bool> EstaActivoAsync(int id, CancellationToken cancelacion) =>
        contexto.Usuarios.AnyAsync(usuario => usuario.Id == id && usuario.Activo, cancelacion);
}
