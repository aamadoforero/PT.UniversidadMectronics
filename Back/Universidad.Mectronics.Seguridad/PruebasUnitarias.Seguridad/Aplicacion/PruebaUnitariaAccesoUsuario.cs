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
/// Control de acceso de las cuentas: estado, rol y restablecimiento de contraseña, con las reglas de la propia cuenta y de la
/// cuenta protegida del superadministrador.
/// </summary>
public sealed class PruebaUnitariaAccesoUsuario
{
    /// <summary>Cuenta del superadministrador, protegida.</summary>
    private readonly Usuario _superAdministrador = new ConstructorUsuario().AsignarId(1).AsignarEmail("sa@universidad.edu")
        .AsignarIdentificacion("1").AsignarRol(FabricaRoles.CrearSuperAdministrador()).MarcarContrasenaDefinitiva().Construir();

    /// <summary>Cuenta del administrador que hace las peticiones.</summary>
    private readonly Usuario _administrador = new ConstructorUsuario().AsignarId(2).AsignarEmail("admin@universidad.edu")
        .AsignarIdentificacion("2").AsignarRol(FabricaRoles.CrearAdministrador()).MarcarContrasenaDefinitiva().Construir();

    /// <summary>Cuenta de estudiante sobre la que se opera.</summary>
    private readonly Usuario _estudiante = new ConstructorUsuario().AsignarId(3).MarcarContrasenaDefinitiva().Construir();

    /// <summary>Servicio bajo prueba.</summary>
    private readonly ServicioAccesoUsuarios _servicio;

    /// <summary>
    /// Arma el servicio con el administrador autenticado.
    /// </summary>
    public PruebaUnitariaAccesoUsuario()
    {
        RepositorioUsuariosEnMemoria repositorio = new RepositorioUsuariosEnMemoria().Sembrar(_superAdministrador, _administrador, _estudiante);
        _servicio = new ServicioAccesoUsuarios(
            new AutorizadorUsuarios(new UsuarioActualFalso().Autenticar(_administrador)), repositorio, new RepositorioRolesEnMemoria(),
            new UnidadTrabajoFalsa(), new ServicioHashFalso(), new GeneradorContrasenaFijo(), new RelojFijo(), new RegistroEventosFalso(),
            new ValidadorCambiarRol(), new ValidadorCambiarEstado());
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
    /// La cuenta del superadministrador está protegida y se informa como inexistente.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarEstadoDelSuperAdministradorDevuelveNoEncontrado()
    {
        Resultado<UsuarioDetalleDto> resultado = await _servicio.CambiarEstadoAsync(_superAdministrador.Id, new(false), CancellationToken.None);

        Assert.Equal(TipoError.NoEncontrado, resultado.Error.Tipo);
        Assert.True(_superAdministrador.Activo);
    }

    /// <summary>
    /// Desactivar una cuenta de estudiante la deja inactiva.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarEstadoDeUnEstudianteLoDesactiva()
    {
        Resultado<UsuarioDetalleDto> resultado = await _servicio.CambiarEstadoAsync(_estudiante.Id, new(false), CancellationToken.None);

        Assert.False(resultado.Valor.Activo);
        Assert.False(_estudiante.PuedeIniciarSesion(FechasPrueba.Inicio));
    }

    /// <summary>
    /// No se puede cambiar de un rol con perfil de estudiante a uno con perfil de profesor.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarRolDeEstudianteAProfesorDevuelveCambioRolNoPermitido()
    {
        Resultado<UsuarioDetalleDto> resultado = await _servicio.CambiarRolAsync(_estudiante.Id, new(FabricaRoles.IdProfesor), CancellationToken.None);

        Assert.Equal("Usuario.CambioRolNoPermitido", resultado.Error.Codigo);
        Assert.Equal("Estudiante", _estudiante.Rol.Nombre);
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
