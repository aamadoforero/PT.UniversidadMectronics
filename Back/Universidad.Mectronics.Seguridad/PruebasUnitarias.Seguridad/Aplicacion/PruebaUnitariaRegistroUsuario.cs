using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Servicios.Usuarios;
using Aplicacion.Seguridad.Validadores.Usuarios;
using Dominio.Seguridad.Entidades;
using PruebasUnitarias.Seguridad.DatosMemoria;
using PruebasUnitarias.Seguridad.Simuladores;
using Universidad.Mectronics.Transversal.Resultados;

namespace PruebasUnitarias.Seguridad.Aplicacion;

/// <summary>
/// Creación de cuentas: permisos, datos académicos según el tipo de perfil del rol, unicidad y rol no asignable.
/// </summary>
public sealed class PruebaUnitariaRegistroUsuario
{
    /// <summary>Cuentas en memoria; ya contiene la del administrador que hace las peticiones.</summary>
    private readonly RepositorioUsuariosEnMemoria _repositorio = new();

    /// <summary>Identidad de quien hace la petición.</summary>
    private readonly UsuarioActualFalso _actor = new();

    /// <summary>Servicio bajo prueba.</summary>
    private readonly ServicioRegistroUsuarios _servicio;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="PruebaUnitariaRegistroUsuario"/>.</summary>
    public PruebaUnitariaRegistroUsuario()
    {
        Usuario administrador = new ConstructorUsuario().AsignarId(1).AsignarEmail("admin@universidad.edu").AsignarIdentificacion("9000")
            .AsignarRol(FabricaRoles.CrearAdministrador()).Construir();
        _repositorio.Sembrar(administrador);
        _actor.Autenticar(administrador);
        _servicio = new ServicioRegistroUsuarios(
            new AutorizadorUsuarios(_actor), _repositorio, new RepositorioRolesEnMemoria(),
            new VerificadorDatosAcademicos(new ConsultaCatalogosFalsa()), new UnidadTrabajoFalsa(), new ServicioHashFalso(),
            new GeneradorContrasenaFijo(), new RelojFijo(), new RegistroEventosFalso(),
            new ValidadorCrearUsuario(), new ValidadorActualizarUsuario());
    }

    /// <summary>
    /// Crea la cuenta con su perfil, entrega la contraseña temporal y guarda solo su hash.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CrearEstudianteValidoCreaLaCuentaConContrasenaTemporal()
    {
        Resultado<UsuarioCreadoDto> resultado = await _servicio.CrearAsync(CrearEstudiante("ana@universidad.edu"), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(GeneradorContrasenaFijo.Contrasena, resultado.Valor.ContrasenaTemporal);
        Assert.Equal(5, resultado.Valor.Usuario.PerfilEstudiante?.UbicacionSemestral);
        Usuario guardado = _repositorio.Usuarios.Single(usuario => usuario.Id == resultado.Valor.Usuario.Id);
        Assert.Equal("hash:" + GeneradorContrasenaFijo.Contrasena, guardado.Contrasena);
        Assert.True(guardado.DebeCambiarContrasena);
    }

    /// <summary>
    /// Un estudiante sin programa ni semestre responde los dos campos faltantes.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CrearEstudianteSinProgramaNiSemestreDevuelveErroresAcademicos()
    {
        CrearUsuarioDto datos = CrearEstudiante("ana@universidad.edu") with { ProgramaId = null, UbicacionSemestral = null };

        Resultado<UsuarioCreadoDto> resultado = await _servicio.CrearAsync(datos, CancellationToken.None);

        Assert.Equal(TipoError.Validacion, resultado.Error.Tipo);
        Assert.Contains("ProgramaId", resultado.Error.Detalles!.Keys);
        Assert.Contains("UbicacionSemestral", resultado.Error.Detalles.Keys);
    }

    /// <summary>
    /// Un correo ya registrado responde un conflicto con el campo afectado, aunque cambien las mayúsculas.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CrearConCorreoDuplicadoDevuelveConflicto()
    {
        Resultado<UsuarioCreadoDto> resultado = await _servicio.CrearAsync(CrearEstudiante("ADMIN@universidad.edu"), CancellationToken.None);

        Assert.Equal(TipoError.Conflicto, resultado.Error.Tipo);
        Assert.Contains("Email", resultado.Error.Detalles!.Keys);
    }

    /// <summary>
    /// Sin el permiso <c>usuarios.crear</c> responde prohibido, aunque la API también lo verifique.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CrearSinPermisoDevuelveProhibido()
    {
        _actor.Autenticar(new ConstructorUsuario().AsignarId(2).Construir());

        Resultado<UsuarioCreadoDto> resultado = await _servicio.CrearAsync(CrearEstudiante("ana@universidad.edu"), CancellationToken.None);

        Assert.Equal(TipoError.Prohibido, resultado.Error.Tipo);
    }

    /// <summary>
    /// Arma los datos de un estudiante válido.
    /// </summary>
    /// <param name="email">Correo de la cuenta.</param>
    /// <returns>Los datos de creación.</returns>
    private static CrearUsuarioDto CrearEstudiante(string email) =>
        new("Ana Pérez", "1012345678", email, FabricaRoles.IdEstudiante, 1, 5, null);
}
