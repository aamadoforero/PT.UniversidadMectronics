using System.Data;
using Aplicacion.Academico.Dtos.Materias;
using Aplicacion.Academico.Servicios.Materias;
using Aplicacion.Academico.Validadores.Materias;
using Dominio.Academico.Autorizacion;
using Dominio.Academico.Entidades;
using PruebasUnitarias.Academico.DatosMemoria;
using PruebasUnitarias.Academico.Simuladores;
using Universidad.Mectronics.Transversal.Resultados;

namespace PruebasUnitarias.Academico.Aplicacion;

/// <summary>
/// Administración de materias: créditos por defecto (R4), máximo de dos materias por profesor (R6) y protección de las
/// inscripciones existentes al editar o desactivar.
/// </summary>
public sealed class PruebaUnitariaMateria
{
    /// <summary>Programa, profesores, materias y estudiantes de prueba.</summary>
    private readonly EscenarioAcademico _escenario = new();

    /// <summary>Unidad de trabajo simulada.</summary>
    private readonly UnidadTrabajoFalsa _unidadTrabajo = new();

    /// <summary>Repositorio de materias simulado.</summary>
    private readonly RepositorioMateriasEnMemoria _materias;

    /// <summary>Profesores del escenario más el profesor 6, que aún no dicta materias.</summary>
    private readonly List<Profesor> _profesores;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="PruebaUnitariaMateria"/>.</summary>
    public PruebaUnitariaMateria()
    {
        _materias = new RepositorioMateriasEnMemoria(new RepositorioEstudiantesEnMemoria(_escenario.Ana), _escenario.Materias);
        _profesores = [.. _escenario.Profesores, EscenarioAcademico.CrearProfesor(6, "Profesor 6")];
    }

    /// <summary>
    /// Toda materia se crea con 3 créditos (R4), activa, dentro de una transacción serializable.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CrearMateriaSiempreTieneTresCreditos()
    {
        Resultado<MateriaDto> resultado = await CrearServicio()
            .CrearAsync(new CrearMateriaDto("  Arquitectura de Software  ", EscenarioAcademico.ProgramaId, 6), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("Arquitectura de Software", resultado.Valor.Nombre);
        Assert.Equal(3, resultado.Valor.Creditos);
        Assert.Equal("Profesor 6", resultado.Valor.Profesor);
        Assert.Equal(11, _materias.Materias.Count);
        Assert.Equal(IsolationLevel.Serializable, _unidadTrabajo.UltimoNivelAislamiento);
    }

    /// <summary>
    /// Un profesor que ya dicta dos materias no puede recibir una tercera (R6).
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CrearMateriaConProfesorQueYaDictaDosDevuelveConflicto()
    {
        Resultado<MateriaDto> resultado = await CrearServicio()
            .CrearAsync(new CrearMateriaDto("Materia nueva", EscenarioAcademico.ProgramaId, 1), CancellationToken.None);

        Assert.Equal("Materia.ProfesorSinCupo", resultado.Error.Codigo);
        Assert.Equal(10, _materias.Materias.Count);
        Assert.Equal(0, _unidadTrabajo.VecesGuardado);
    }

    /// <summary>
    /// El nombre se compara sin distinguir mayúsculas dentro del programa.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CrearMateriaConNombreRepetidoDevuelveConflicto()
    {
        Resultado<MateriaDto> resultado = await CrearServicio()
            .CrearAsync(new CrearMateriaDto("materia 01", EscenarioAcademico.ProgramaId, 6), CancellationToken.None);

        Assert.Equal("Materia.NombreDuplicado", resultado.Error.Codigo);
    }

    /// <summary>
    /// Una materia con estudiantes inscritos no se puede desactivar.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task DesactivarMateriaConInscritosDevuelveConflicto()
    {
        _escenario.Ana.Inscribir(_escenario.ObtenerMateria(1), FechasPrueba.Inicio);

        Resultado<MateriaDto> resultado = await CrearServicio().CambiarEstadoAsync(1, new CambiarEstadoMateriaDto(false), CancellationToken.None);

        Assert.Equal("Materia.ConInscritos", resultado.Error.Codigo);
        Assert.True(_escenario.ObtenerMateria(1).Activo);
    }

    /// <summary>
    /// Cambiar el profesor de una materia con inscritos podría dejar a un estudiante con dos clases del mismo profesor (R7).
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarProfesorDeMateriaConInscritosDevuelveConflicto()
    {
        _escenario.Ana.Inscribir(_escenario.ObtenerMateria(1), FechasPrueba.Inicio);

        Resultado<MateriaDto> resultado = await CrearServicio()
            .ActualizarAsync(1, new ActualizarMateriaDto("Materia 01", 6), CancellationToken.None);

        Assert.Equal("Materia.CambioProfesorConInscritos", resultado.Error.Codigo);
        Assert.Equal(1, _escenario.ObtenerMateria(1).ProfesorId);
    }

    /// <summary>
    /// Las materias desactivadas solo las consulta quien tiene <c>materias.gestionar</c>.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task ConsultarMateriasInactivasSinPermisoDevuelveProhibido()
    {
        Resultado<IReadOnlyList<MateriaDto>> resultado = await CrearServicio(CodigosPermisoInscripciones.Propias)
            .ObtenerTodasAsync(true, CancellationToken.None);

        Assert.Equal(TipoError.Prohibido, resultado.Error.Tipo);
    }

    /// <summary>
    /// Crea el servicio para una cuenta administradora.
    /// </summary>
    /// <param name="permisos">Permisos de la sesión; por defecto, <c>materias.gestionar</c>.</param>
    /// <returns>El servicio bajo prueba.</returns>
    private ServicioMaterias CrearServicio(params string[] permisos) => new(
        new UsuarioActualFalso(1, permisos.Length == 0 ? [CodigosPermisoMaterias.Gestionar] : permisos),
        _materias,
        new RepositorioProfesoresEnMemoria(_profesores),
        new RepositorioProgramasEnMemoria(EscenarioAcademico.ProgramaId),
        _unidadTrabajo,
        new RelojFijo(),
        new RegistroEventosFalso(),
        new ValidadorCrearMateria(),
        new ValidadorActualizarMateria(),
        new ValidadorCambiarEstadoMateria());
}
