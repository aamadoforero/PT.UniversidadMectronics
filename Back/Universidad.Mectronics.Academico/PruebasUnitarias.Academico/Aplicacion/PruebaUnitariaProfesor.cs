using Aplicacion.Academico.Dtos.Profesores;
using Aplicacion.Academico.Servicios.Profesores;
using Dominio.Academico.Autorizacion;
using PruebasUnitarias.Academico.DatosMemoria;
using PruebasUnitarias.Academico.Simuladores;
using Universidad.Mectronics.Transversal.Resultados;

namespace PruebasUnitarias.Academico.Aplicacion;

/// <summary>
/// Materias dictadas: el profesor autenticado ve solo sus clases, con los nombres de sus inscritos.
/// </summary>
public sealed class PruebaUnitariaProfesor
{
    /// <summary>Programa, profesores, materias y estudiantes de prueba.</summary>
    private readonly EscenarioAcademico _escenario = new();

    /// <summary>
    /// El profesor 1 (cuenta 201) ve sus dos materias con sus inscritos, y no las de los demás profesores.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task ObtenerMateriasDictadasDevuelveSoloSusMateriasConLosInscritos()
    {
        _escenario.Ana.Inscribir(_escenario.ObtenerMateria(1), FechasPrueba.Inicio);
        _escenario.Bruno.Inscribir(_escenario.ObtenerMateria(1), FechasPrueba.Inicio);
        _escenario.Carla.Inscribir(_escenario.ObtenerMateria(3), FechasPrueba.Inicio);

        Resultado<IReadOnlyList<MateriaDictadaDto>> resultado = await CrearServicio(201).ObtenerMateriasDictadasAsync(CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal([1, 2], resultado.Valor.Select(materia => materia.MateriaId));
        Assert.Equal(["Ana Torres", "Bruno Díaz"], resultado.Valor[0].Estudiantes);
        Assert.Equal(2, resultado.Valor[0].TotalInscritos);
        Assert.Empty(resultado.Valor[1].Estudiantes);
    }

    /// <summary>
    /// Una cuenta sin perfil de profesor no tiene materias dictadas.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task ObtenerMateriasDictadasSinPerfilDeProfesorDevuelveProhibido()
    {
        Resultado<IReadOnlyList<MateriaDictadaDto>> resultado = await CrearServicio(101).ObtenerMateriasDictadasAsync(CancellationToken.None);

        Assert.Equal("Profesor.SinPerfilProfesor", resultado.Error.Codigo);
        Assert.Equal(TipoError.Prohibido, resultado.Error.Tipo);
    }

    /// <summary>
    /// Crea el servicio para una cuenta con el permiso de materias dictadas.
    /// </summary>
    /// <param name="usuarioId">Cuenta autenticada.</param>
    /// <returns>El servicio bajo prueba.</returns>
    private ServicioProfesores CrearServicio(int usuarioId)
    {
        RepositorioEstudiantesEnMemoria estudiantes = new(_escenario.Ana, _escenario.Bruno, _escenario.Carla);
        return new ServicioProfesores(
            new UsuarioActualFalso(usuarioId, CodigosPermisoMaterias.Dictadas),
            new RepositorioProfesoresEnMemoria(_escenario.Profesores),
            new RepositorioMateriasEnMemoria(estudiantes, _escenario.Materias),
            estudiantes);
    }
}
