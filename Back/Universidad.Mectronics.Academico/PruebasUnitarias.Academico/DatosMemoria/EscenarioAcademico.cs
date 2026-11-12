using Dominio.Academico.Entidades;

namespace PruebasUnitarias.Academico.DatosMemoria;

/// <summary>
/// Datos de prueba equivalentes a <c>Database/DML.sql</c>: un programa, 5 profesores, 10 materias y cuatro estudiantes.
/// </summary>
public sealed class EscenarioAcademico
{
    /// <summary>Programa de los estudiantes y las materias.</summary>
    public const int ProgramaId = 1;

    /// <summary>Programa ajeno, para probar materias de otro programa.</summary>
    public const int OtroProgramaId = 2;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="EscenarioAcademico"/>.</summary>
    public EscenarioAcademico()
    {
        Programa programa = FabricaEntidades.Crear<Programa>().Asignar(nameof(Programa.Id), ProgramaId)
            .Asignar(nameof(Programa.Nombre), "Ingeniería de Sistemas").Asignar(nameof(Programa.Activo), true);

        Profesores = Enumerable.Range(1, 5).Select(numero => CrearProfesor(numero, $"Profesor {numero}")).ToList();
        Materias = Enumerable.Range(1, 10)
            .Select(numero => CrearMateria(numero, $"Materia {numero:00}", Profesores[(numero - 1) / 2], ProgramaId))
            .ToList();

        Ana = CrearEstudiante(1, 101, "Ana Torres", programa);
        Bruno = CrearEstudiante(2, 102, "Bruno Díaz", programa);
        Carla = CrearEstudiante(3, 103, "Carla Ruiz", programa);
    }

    /// <summary>Profesores 1 a 5.</summary>
    public IReadOnlyList<Profesor> Profesores { get; }

    /// <summary>Materias 1 a 10.</summary>
    public IReadOnlyList<Materia> Materias { get; }

    /// <summary>Estudiante principal de las pruebas (cuenta 101).</summary>
    public Estudiante Ana { get; }

    /// <summary>Compañero (cuenta 102).</summary>
    public Estudiante Bruno { get; }

    /// <summary>Compañera (cuenta 103).</summary>
    public Estudiante Carla { get; }

    /// <summary>
    /// Obtiene una materia por su número.
    /// </summary>
    /// <param name="numero">Número de la materia, de 1 a 10.</param>
    /// <returns>La materia.</returns>
    public Materia ObtenerMateria(int numero) => Materias[numero - 1];

    /// <summary>
    /// Crea una materia de 3 créditos con identificador y profesor asignados, como si viniera de la base de datos.
    /// </summary>
    /// <param name="id">Identificador.</param>
    /// <param name="nombre">Nombre.</param>
    /// <param name="profesor">Profesor que la dicta.</param>
    /// <param name="programaId">Programa.</param>
    /// <returns>La materia activa.</returns>
    public static Materia CrearMateria(int id, string nombre, Profesor profesor, int programaId)
    {
        ArgumentNullException.ThrowIfNull(profesor);
        return Materia.Crear(nombre, programaId, profesor.Id, FechasPrueba.Inicio)
            .Asignar(nameof(Materia.Id), id)
            .Asignar(nameof(Materia.Profesor), profesor);
    }

    /// <summary>
    /// Crea un profesor con cuenta activa.
    /// </summary>
    /// <param name="id">Identificador del perfil.</param>
    /// <param name="nombre">Nombre.</param>
    /// <returns>El profesor.</returns>
    public static Profesor CrearProfesor(int id, string nombre) => FabricaEntidades.Crear<Profesor>()
        .Asignar(nameof(Profesor.Id), id)
        .Asignar(nameof(Profesor.UsuarioId), 200 + id)
        .Asignar(nameof(Profesor.Usuario), CrearUsuario(200 + id, nombre));

    /// <summary>
    /// Crea un estudiante con cuenta activa y sin inscripciones.
    /// </summary>
    /// <param name="id">Identificador del perfil.</param>
    /// <param name="usuarioId">Identificador de la cuenta.</param>
    /// <param name="nombre">Nombre.</param>
    /// <param name="programa">Programa.</param>
    /// <returns>El estudiante.</returns>
    private static Estudiante CrearEstudiante(int id, int usuarioId, string nombre, Programa programa) => FabricaEntidades.Crear<Estudiante>()
        .Asignar(nameof(Estudiante.Id), id)
        .Asignar(nameof(Estudiante.UsuarioId), usuarioId)
        .Asignar(nameof(Estudiante.Usuario), CrearUsuario(usuarioId, nombre))
        .Asignar(nameof(Estudiante.ProgramaId), programa.Id)
        .Asignar(nameof(Estudiante.Programa), programa)
        .Asignar(nameof(Estudiante.UbicacionSemestral), 1);

    /// <summary>
    /// Crea una cuenta activa.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="nombre">Nombre completo.</param>
    /// <returns>La cuenta.</returns>
    private static Usuario CrearUsuario(int id, string nombre) => FabricaEntidades.Crear<Usuario>()
        .Asignar(nameof(Usuario.Id), id)
        .Asignar(nameof(Usuario.NombreCompleto), nombre)
        .Asignar(nameof(Usuario.Activo), true);
}
