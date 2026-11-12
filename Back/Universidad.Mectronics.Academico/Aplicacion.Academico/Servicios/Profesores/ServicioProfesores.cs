using Aplicacion.Academico.Dtos.Profesores;
using Aplicacion.Academico.Errores;
using Aplicacion.Academico.Interfaces;
using Aplicacion.Academico.Mapeos;
using Dominio.Academico.Entidades;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Servicios.Profesores;

/// <summary>
/// Implementación de la consulta de profesores. Las materias dictadas se toman de la cuenta autenticada.
/// </summary>
/// <param name="usuarioActual">Cuenta autenticada.</param>
/// <param name="repositorioProfesores">Acceso a los profesores.</param>
/// <param name="repositorioMaterias">Acceso a las materias.</param>
/// <param name="repositorioEstudiantes">Acceso a los estudiantes, para los inscritos de cada materia.</param>
public sealed class ServicioProfesores(
    IUsuarioActual usuarioActual,
    IRepositorioProfesores repositorioProfesores,
    IRepositorioMaterias repositorioMaterias,
    IRepositorioEstudiantes repositorioEstudiantes) : IServicioProfesores
{
    /// <summary>
    /// Obtiene los profesores y cuántas materias activas dicta cada uno.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Los profesores, ordenados por nombre.</returns>
    public async Task<Resultado<IReadOnlyList<ProfesorDto>>> ObtenerTodosAsync(CancellationToken cancelacion)
    {
        IReadOnlyList<Profesor> profesores = await repositorioProfesores.ObtenerTodosAsync(cancelacion);
        IReadOnlyDictionary<int, int> conteo = await repositorioMaterias.ObtenerConteoActivasPorProfesorAsync(cancelacion);
        IReadOnlyList<ProfesorDto> dtos = profesores
            .Select(profesor => profesor.ConvertirAProfesorDto(conteo.GetValueOrDefault(profesor.Id)))
            .ToList();
        return Resultado.CrearExito(dtos);
    }

    /// <summary>
    /// Obtiene las materias activas del profesor autenticado con los nombres de sus inscritos.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Las materias dictadas, o un error si la cuenta no tiene perfil de profesor.</returns>
    public async Task<Resultado<IReadOnlyList<MateriaDictadaDto>>> ObtenerMateriasDictadasAsync(CancellationToken cancelacion)
    {
        Profesor? profesor = await repositorioProfesores.ObtenerPorUsuarioIdAsync(usuarioActual.Id, cancelacion);
        if (profesor is null)
        {
            return ErroresProfesor.CrearSinPerfilProfesor();
        }

        IReadOnlyList<Materia> materias = await repositorioMaterias.ObtenerActivasPorProfesorAsync(profesor.Id, cancelacion);
        List<MateriaDictadaDto> dictadas = [];
        foreach (Materia materia in materias)
        {
            IReadOnlyList<string> estudiantes = await repositorioEstudiantes.ObtenerNombresInscritosAsync(materia.Id, 0, cancelacion);
            dictadas.Add(new MateriaDictadaDto(materia.Id, materia.Nombre, materia.Creditos, estudiantes.Count, estudiantes));
        }

        return dictadas;
    }
}
