using System.Data;
using Aplicacion.Academico.Dtos.Inscripciones;
using Aplicacion.Academico.Servicios.Inscripciones;
using Aplicacion.Academico.Validadores.Inscripciones;
using Dominio.Academico.Autorizacion;
using Dominio.Academico.Entidades;
using PruebasUnitarias.Academico.DatosMemoria;
using PruebasUnitarias.Academico.Simuladores;
using Universidad.Mectronics.Transversal.Resultados;

namespace PruebasUnitarias.Academico.Aplicacion;

/// <summary>
/// Inscripción de materias: créditos (R2, R4), máximo de tres materias (R5), profesor no repetido (R7), cancelación y
/// compañeros de clase (R9).
/// </summary>
public sealed class PruebaUnitariaInscripcion
{
    /// <summary>Programa, profesores, materias y estudiantes de prueba.</summary>
    private readonly EscenarioAcademico _escenario = new();

    /// <summary>Unidad de trabajo simulada.</summary>
    private readonly UnidadTrabajoFalsa _unidadTrabajo = new();

    /// <summary>Repositorio de estudiantes simulado.</summary>
    private readonly RepositorioEstudiantesEnMemoria _estudiantes;

    /// <summary>Repositorio de materias simulado.</summary>
    private readonly RepositorioMateriasEnMemoria _materias;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="PruebaUnitariaInscripcion"/>.</summary>
    public PruebaUnitariaInscripcion()
    {
        _estudiantes = new RepositorioEstudiantesEnMemoria(_escenario.Ana, _escenario.Bruno, _escenario.Carla);
        _materias = new RepositorioMateriasEnMemoria(_estudiantes, _escenario.Materias);
        _materias.Materias.Add(EscenarioAcademico.CrearMateria(11, "Materia de cuatro créditos", _escenario.Profesores[3], EscenarioAcademico.ProgramaId)
            .Asignar(nameof(Materia.Creditos), 4));
        _materias.Materias.Add(EscenarioAcademico.CrearMateria(12, "Materia de otro programa", _escenario.Profesores[4], EscenarioAcademico.OtroProgramaId));
    }

    /// <summary>
    /// Una materia válida se inscribe dentro de una transacción serializable y suma sus 3 créditos.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task InscribirMateriaValidaGuardaEnTransaccionSerializableYSumaTresCreditos()
    {
        Resultado<InscripcionDto> resultado = await CrearServicio().InscribirAsync(new InscribirMateriaDto(1), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("Profesor 1", resultado.Valor.Profesor);
        Assert.Equal(3, _escenario.Ana.CreditosInscritos);
        Assert.Equal(IsolationLevel.Serializable, _unidadTrabajo.UltimoNivelAislamiento);
        Assert.Equal(1, _unidadTrabajo.VecesGuardado);
    }

    /// <summary>
    /// Con tres materias inscritas (9 créditos), la cuarta responde conflicto y no se guarda (R5).
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task InscribirCuartaMateriaDevuelveConflictoPorMaximoDeMaterias()
    {
        InscribirPrevias(_escenario.Ana, 1, 3, 5);

        Resultado<InscripcionDto> resultado = await CrearServicio().InscribirAsync(new InscribirMateriaDto(7), CancellationToken.None);

        Assert.Equal("Inscripcion.MaximoMaterias", resultado.Error.Codigo);
        Assert.Equal(TipoError.Conflicto, resultado.Error.Tipo);
        Assert.Equal(9, _escenario.Ana.CreditosInscritos);
        Assert.Equal(0, _unidadTrabajo.VecesGuardado);
    }

    /// <summary>
    /// La segunda materia del mismo profesor responde conflicto (R7): las materias 1 y 2 las dicta el profesor 1.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task InscribirSegundaMateriaDelMismoProfesorDevuelveConflicto()
    {
        InscribirPrevias(_escenario.Ana, 1);

        Resultado<InscripcionDto> resultado = await CrearServicio().InscribirAsync(new InscribirMateriaDto(2), CancellationToken.None);

        Assert.Equal("Inscripcion.ProfesorRepetido", resultado.Error.Codigo);
        Assert.Single(_escenario.Ana.InscripcionesActivas);
    }

    /// <summary>
    /// Con 6 créditos inscritos, una materia de 4 superaría el máximo de 9 créditos del programa, aunque solo sea la tercera materia.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task InscribirMateriaQueSuperaNueveCreditosDevuelveConflicto()
    {
        InscribirPrevias(_escenario.Ana, 1, 3);

        Resultado<InscripcionDto> resultado = await CrearServicio().InscribirAsync(new InscribirMateriaDto(11), CancellationToken.None);

        Assert.Equal("Inscripcion.MaximoCreditos", resultado.Error.Codigo);
        Assert.Equal(6, _escenario.Ana.CreditosInscritos);
    }

    /// <summary>
    /// Una materia ya inscrita, desactivada, de otro programa o inexistente responde el error correspondiente y no guarda nada.
    /// </summary>
    /// <param name="materiaId">Materia solicitada.</param>
    /// <param name="codigoEsperado">Código del error.</param>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Theory]
    [InlineData(1, "Inscripcion.MateriaYaInscrita")]
    [InlineData(9, "Inscripcion.MateriaInactiva")]
    [InlineData(12, "Inscripcion.MateriaDeOtroPrograma")]
    [InlineData(99, "Inscripcion.MateriaNoEncontrada")]
    public async Task InscribirMateriaNoDisponibleDevuelveErrorSinGuardar(int materiaId, string codigoEsperado)
    {
        InscribirPrevias(_escenario.Ana, 1);
        _escenario.ObtenerMateria(9).CambiarEstado(false, FechasPrueba.Inicio);

        Resultado<InscripcionDto> resultado = await CrearServicio().InscribirAsync(new InscribirMateriaDto(materiaId), CancellationToken.None);

        Assert.Equal(codigoEsperado, resultado.Error.Codigo);
        Assert.Equal(0, _unidadTrabajo.VecesGuardado);
    }

    /// <summary>
    /// Cancelar una de tres materias libera el cupo: la inscripción cancelada queda como historial y se puede inscribir otra.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CancelarInscripcionLiberaElCupoParaOtraMateria()
    {
        InscribirPrevias(_escenario.Ana, 1, 3, 5);
        ServicioInscripciones servicio = CrearServicio();

        Resultado cancelacion = await servicio.CancelarAsync(5, CancellationToken.None);
        Resultado<InscripcionDto> inscripcion = await servicio.InscribirAsync(new InscribirMateriaDto(7), CancellationToken.None);

        Assert.True(cancelacion.EsExitoso);
        Assert.True(inscripcion.EsExitoso);
        Assert.Equal(4, _escenario.Ana.Inscripciones.Count);
        Assert.Equal([1, 3, 7], _escenario.Ana.InscripcionesActivas.Select(actual => actual.MateriaId).Order());
        Assert.Equal(9, _escenario.Ana.CreditosInscritos);
    }

    /// <summary>
    /// Los compañeros de una clase son solo los nombres de los demás inscritos en ella (R9).
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task ObtenerCompanerosDevuelveSoloLosNombresDeLosDemasInscritos()
    {
        InscribirPrevias(_escenario.Ana, 1);
        InscribirPrevias(_escenario.Bruno, 1, 3);
        InscribirPrevias(_escenario.Carla, 3);

        Resultado<CompanerosClaseDto> resultado = await CrearServicio().ObtenerCompanerosAsync(1, CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("Materia 01", resultado.Valor.Materia);
        Assert.Equal(["Bruno Díaz"], resultado.Valor.Companeros);
    }

    /// <summary>
    /// Los compañeros de una materia que el estudiante no tiene inscrita no se pueden consultar.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task ObtenerCompanerosDeMateriaNoInscritaDevuelveProhibido()
    {
        InscribirPrevias(_escenario.Bruno, 3);

        Resultado<CompanerosClaseDto> resultado = await CrearServicio().ObtenerCompanerosAsync(3, CancellationToken.None);

        Assert.Equal("Inscripcion.CompanerosNoPermitidos", resultado.Error.Codigo);
        Assert.Equal(TipoError.Prohibido, resultado.Error.Tipo);
    }

    /// <summary>
    /// Una cuenta sin perfil de estudiante no puede inscribir.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task InscribirSinPerfilDeEstudianteDevuelveProhibido()
    {
        Resultado<InscripcionDto> resultado = await CrearServicio(999).InscribirAsync(new InscribirMateriaDto(1), CancellationToken.None);

        Assert.Equal("Inscripcion.SinPerfilEstudiante", resultado.Error.Codigo);
    }

    /// <summary>
    /// Inscribe materias directamente en el dominio, como si ya estuvieran guardadas.
    /// </summary>
    /// <param name="estudiante">Estudiante.</param>
    /// <param name="materias">Números de las materias.</param>
    private void InscribirPrevias(Estudiante estudiante, params int[] materias)
    {
        foreach (int numero in materias)
        {
            estudiante.Inscribir(_escenario.ObtenerMateria(numero), FechasPrueba.Inicio);
        }
    }

    /// <summary>
    /// Crea el servicio para una cuenta con permiso de inscripciones propias.
    /// </summary>
    /// <param name="usuarioId">Cuenta autenticada; por defecto, la de Ana.</param>
    /// <returns>El servicio bajo prueba.</returns>
    private ServicioInscripciones CrearServicio(int usuarioId = 101) => new(
        new UsuarioActualFalso(usuarioId, CodigosPermisoInscripciones.Propias),
        _estudiantes,
        _materias,
        _unidadTrabajo,
        new RelojFijo(),
        new RegistroEventosFalso(),
        new ValidadorInscribirMateria());
}
