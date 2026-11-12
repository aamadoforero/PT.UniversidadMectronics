using Aplicacion.Seguridad.Interfaces;
using Dominio.Seguridad.Entidades;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>Repositorio de inscripciones en memoria.</summary>
public sealed class RepositorioInscripcionesEnMemoria : IRepositorioInscripciones
{
    /// <summary>Inscripciones guardadas, vigentes o canceladas.</summary>
    public List<Inscripcion> Inscripciones { get; } = [];

    /// <summary>
    /// Agrega inscripciones vigentes de un estudiante.
    /// </summary>
    /// <param name="estudianteId">Perfil de estudiante dueño de las inscripciones.</param>
    /// <param name="cantidad">Cuántas inscripciones agregar.</param>
    /// <returns>El mismo repositorio, para encadenar.</returns>
    public RepositorioInscripcionesEnMemoria Sembrar(int estudianteId, int cantidad)
    {
        for (int numero = 0; numero < cantidad; numero++)
        {
            Inscripciones.Add(new Inscripcion(estudianteId));
        }

        return this;
    }

    /// <summary>
    /// Obtiene las inscripciones vigentes de un estudiante.
    /// </summary>
    /// <param name="estudianteId">Identificador del perfil de estudiante.</param>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Las inscripciones vigentes.</returns>
    public Task<IReadOnlyList<Inscripcion>> ObtenerVigentesAsync(int estudianteId, CancellationToken cancelacion)
    {
        IReadOnlyList<Inscripcion> vigentes = Inscripciones
            .Where(inscripcion => inscripcion.EstudianteId == estudianteId && inscripcion.Activo)
            .ToList();
        return Task.FromResult(vigentes);
    }
}
