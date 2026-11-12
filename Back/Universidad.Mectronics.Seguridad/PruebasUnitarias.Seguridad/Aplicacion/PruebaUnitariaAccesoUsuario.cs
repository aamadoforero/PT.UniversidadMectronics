using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Servicios.Usuarios;
using Aplicacion.Seguridad.Validadores.Usuarios;
using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.Politicas;
using PruebasUnitarias.Seguridad.DatosMemoria;
using PruebasUnitarias.Seguridad.Simuladores;
using Universidad.Mectronics.Transversal.Resultados;

namespace PruebasUnitarias.Seguridad.Aplicacion;

/// <summary>
/// Control de acceso de las cuentas: estado y restablecimiento de contraseña, con las reglas de la propia cuenta.
/// </summary>
public sealed class PruebaUnitariaAccesoUsuario
{
    /// <summary>Cuenta del administrador que hace las peticiones.</summary>
    private readonly Usuario _administrador = new ConstructorUsuario().AsignarId(2).AsignarEmail("admin@universidad.edu")
        .AsignarIdentificacion("2").AsignarRol(FabricaRoles.CrearAdministrador()).MarcarContrasenaDefinitiva().Construir();

    /// <summary>Cuenta de estudiante sobre la que se opera.</summary>
    private readonly Usuario _estudiante = new ConstructorUsuario().AsignarId(3).MarcarContrasenaDefinitiva().Construir();

    /// <summary>Cuenta de profesor sobre la que se opera.</summary>
    private readonly Usuario _profesor = new ConstructorUsuario().AsignarId(4).AsignarEmail("profe@universidad.edu")
        .AsignarIdentificacion("4").AsignarRol(FabricaRoles.CrearProfesor()).MarcarContrasenaDefinitiva().Construir();

    /// <summary>Inscripciones en memoria: el estudiante tiene dos vigentes.</summary>
    private readonly RepositorioInscripcionesEnMemoria _inscripciones;

    /// <summary>Servicio bajo prueba.</summary>
    private readonly ServicioAccesoUsuarios _servicio;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="PruebaUnitariaAccesoUsuario"/>.</summary>
    public PruebaUnitariaAccesoUsuario()
    {
        _inscripciones = new RepositorioInscripcionesEnMemoria().Sembrar(_estudiante.PerfilEstudiante!.Id, 2);
        RepositorioUsuariosEnMemoria repositorio = new RepositorioUsuariosEnMemoria().Sembrar(_administrador, _estudiante, _profesor);
        _servicio = new ServicioAccesoUsuarios(
            new AutorizadorUsuarios(new UsuarioActualFalso().Autenticar(_administrador)), repositorio, _inscripciones,
            new UnidadTrabajoFalsa(), new ServicioHashFalso(), new GeneradorContrasenaFijo(), new RelojFijo(), new RegistroEventosFalso(),
            new ValidadorCambiarEstado());
    }

    /// <summary>
    /// Nadie puede desactivar su propia cuenta.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarEstadoDeLaPropiaCuentaDevuelveConflicto()
    {
        Resultado<UsuarioDetalleDto> resultado = await _servicio.CambiarEstadoAsync(_administrador.Id, new(false), CancellationToken.None);

        Assert.Equal("Usuario.OperacionSobreCuentaPropia", resultado.Error.Codigo);
        Assert.True(_administrador.Activo);
    }

    /// <summary>
    /// Desactivar una cuenta de estudiante la deja inactiva y cancela sus inscripciones vigentes.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarEstadoDeUnEstudianteLoDesactivaYCancelaSusInscripciones()
    {
        Resultado<UsuarioDetalleDto> resultado = await _servicio.CambiarEstadoAsync(_estudiante.Id, new(false), CancellationToken.None);

        Assert.False(resultado.Valor.Activo);
        Assert.False(_estudiante.PuedeIniciarSesion(FechasPrueba.Inicio));
        Assert.All(_inscripciones.Inscripciones, inscripcion => Assert.False(inscripcion.Activo));
        Assert.All(_inscripciones.Inscripciones, inscripcion => Assert.Equal(FechasPrueba.Inicio, inscripcion.FechaModificacion));
    }

    /// <summary>
    /// Desactivar una cuenta sin perfil de estudiante no toca inscripciones.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarEstadoDeUnProfesorNoCancelaInscripciones()
    {
        Resultado<UsuarioDetalleDto> resultado = await _servicio.CambiarEstadoAsync(_profesor.Id, new(false), CancellationToken.None);

        Assert.False(resultado.Valor.Activo);
        Assert.All(_inscripciones.Inscripciones, inscripcion => Assert.True(inscripcion.Activo));
    }

    /// <summary>
    /// Reactivar un estudiante no restaura las inscripciones canceladas al desactivarlo.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarEstadoAlReactivarUnEstudianteNoRestauraSusInscripciones()
    {
        await _servicio.CambiarEstadoAsync(_estudiante.Id, new(false), CancellationToken.None);

        Resultado<UsuarioDetalleDto> resultado = await _servicio.CambiarEstadoAsync(_estudiante.Id, new(true), CancellationToken.None);

        Assert.True(resultado.Valor.Activo);
        Assert.All(_inscripciones.Inscripciones, inscripcion => Assert.False(inscripcion.Activo));
    }

    /// <summary>
    /// Restablecer la contraseña la deja temporal, desbloquea la cuenta y entrega la contraseña nueva una sola vez.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task RestablecerContrasenaDejaLaContrasenaTemporalYDesbloquea()
    {
        for (int intento = 1; intento <= 5; intento++)
        {
            _estudiante.RegistrarInicioFallido(FechasPrueba.Inicio, PoliticaBloqueo.Predeterminada);
        }

        Resultado<ContrasenaRestablecidaDto> resultado = await _servicio.RestablecerContrasenaAsync(_estudiante.Id, CancellationToken.None);

        Assert.Equal(GeneradorContrasenaFijo.Contrasena, resultado.Valor.ContrasenaTemporal);
        Assert.True(_estudiante.DebeCambiarContrasena);
        Assert.False(_estudiante.EstaBloqueado(FechasPrueba.Inicio));
    }
}
