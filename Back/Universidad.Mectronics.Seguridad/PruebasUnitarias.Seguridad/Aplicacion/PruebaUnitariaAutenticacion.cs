using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Servicios.Autenticacion;
using Aplicacion.Seguridad.Validadores.Autenticacion;
using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.Politicas;
using PruebasUnitarias.Seguridad.DatosMemoria;
using PruebasUnitarias.Seguridad.Simuladores;
using Universidad.Mectronics.Transversal.Resultados;

namespace PruebasUnitarias.Seguridad.Aplicacion;

/// <summary>
/// Inicio y renovación de sesión: credenciales, bloqueo, respuesta uniforme ante correos inexistentes e invalidación de
/// sesiones por cambio de contraseña o desactivación.
/// </summary>
public sealed class PruebaUnitariaAutenticacion
{
    /// <summary>Cuentas en memoria.</summary>
    private readonly RepositorioUsuariosEnMemoria _repositorio = new();

    /// <summary>Hash simulado, que registra cada verificación.</summary>
    private readonly ServicioHashFalso _hash = new();

    /// <summary>Unidad de trabajo simulada.</summary>
    private readonly UnidadTrabajoFalsa _unidadTrabajo = new();

    /// <summary>Servicio bajo prueba.</summary>
    private readonly ServicioAutenticacion _servicio;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="PruebaUnitariaAutenticacion"/>.</summary>
    public PruebaUnitariaAutenticacion()
    {
        ServicioTokensFalso tokens = new();
        _repositorio.Sembrar(new ConstructorUsuario().AsignarContrasena("Clave#2026").Construir());
        _servicio = new ServicioAutenticacion(
            _repositorio, _unidadTrabajo, _hash, tokens, new RelojFijo(), new RegistroEventosFalso(),
            PoliticaBloqueo.Predeterminada, new ValidadorIniciarSesion(), new GeneradorSesion(tokens));
    }

    /// <summary>
    /// Con credenciales correctas se crea la sesión, aunque el correo llegue con mayúsculas y espacios.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task IniciarSesionConCredencialesCorrectasCreaLaSesion()
    {
        Resultado<SesionCreadaDto> resultado = await _servicio.IniciarSesionAsync(new(" ANA.PEREZ@universidad.edu ", "Clave#2026"), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("ana.perez@universidad.edu", resultado.Valor.Usuario.Email);
        Assert.Contains("inscripciones.propias", resultado.Valor.Usuario.Permisos);
    }

    /// <summary>
    /// Una contraseña incorrecta cuenta como intento fallido, se guarda y responde el error genérico.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task IniciarSesionConContrasenaIncorrectaRegistraElIntentoYRespondeCredencialesInvalidas()
    {
        Resultado<SesionCreadaDto> resultado = await _servicio.IniciarSesionAsync(new("ana.perez@universidad.edu", "Otra#2026"), CancellationToken.None);

        Assert.Equal("Autenticacion.CredencialesInvalidas", resultado.Error.Codigo);
        Assert.Equal(1, _repositorio.Usuarios[0].IntentosInicioFallidos);
        Assert.Equal(1, _unidadTrabajo.VecesGuardado);
    }

    /// <summary>
    /// Los cinco intentos fallidos responden el error genérico; a partir del sexto la cuenta está bloqueada y responde
    /// cuenta bloqueada, incluso con la contraseña correcta.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task IniciarSesionTrasCincoIntentosFallidosRespondeCuentaBloqueadaAunConLaContrasenaCorrecta()
    {
        for (int intento = 1; intento <= 5; intento++)
        {
            Resultado<SesionCreadaDto> fallido = await _servicio.IniciarSesionAsync(new("ana.perez@universidad.edu", "Otra#2026"), CancellationToken.None);
            Assert.Equal("Autenticacion.CredencialesInvalidas", fallido.Error.Codigo);
        }

        Resultado<SesionCreadaDto> resultado = await _servicio.IniciarSesionAsync(new("ana.perez@universidad.edu", "Clave#2026"), CancellationToken.None);

        Assert.Equal("Autenticacion.CuentaBloqueada", resultado.Error.Codigo);
        Assert.Equal(TipoError.DemasiadosIntentos, resultado.Error.Tipo);
        Assert.NotNull(_repositorio.Usuarios[0].BloqueoHasta);
    }

    /// <summary>
    /// Un correo inexistente responde el mismo error que una contraseña incorrecta y verifica contra el hash de referencia,
    /// para no revelar por el tiempo de respuesta qué correos están registrados.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task IniciarSesionConCorreoInexistenteVerificaElHashDeReferenciaYRespondeElMismoError()
    {
        Resultado<SesionCreadaDto> resultado = await _servicio.IniciarSesionAsync(new("nadie@universidad.edu", "Clave#2026"), CancellationToken.None);

        Assert.Equal("Autenticacion.CredencialesInvalidas", resultado.Error.Codigo);
        Assert.Equal([_hash.HashReferencia], _hash.HashesVerificados);
    }

    /// <summary>
    /// Sin correo ni contraseña responde un error de validación con el detalle de ambos campos, sin consultar la cuenta.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task IniciarSesionConDatosVaciosDevuelveErroresPorCampo()
    {
        Resultado<SesionCreadaDto> resultado = await _servicio.IniciarSesionAsync(new(null, " "), CancellationToken.None);

        Assert.Equal(TipoError.Validacion, resultado.Error.Tipo);
        Assert.Contains("Email", resultado.Error.Detalles!.Keys);
        Assert.Contains("Contrasena", resultado.Error.Detalles.Keys);
        Assert.Empty(_hash.HashesVerificados);
    }

    /// <summary>
    /// Un token de renovación emitido antes de un cambio de contraseña deja de ser válido.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task RenovarSesionTrasCambiarLaContrasenaRechazaElTokenAnterior()
    {
        Resultado<SesionCreadaDto> sesion = await _servicio.IniciarSesionAsync(new("ana.perez@universidad.edu", "Clave#2026"), CancellationToken.None);
        _repositorio.Usuarios[0].CambiarContrasena("hash:Nueva#2026", FechasPrueba.Inicio);

        Resultado<SesionCreadaDto> renovacion = await _servicio.RenovarSesionAsync(new(sesion.Valor.TokenRenovacion.Valor), CancellationToken.None);

        Assert.Equal("Autenticacion.SesionInvalida", renovacion.Error.Codigo);
    }

    /// <summary>
    /// Una cuenta desactivada no puede renovar su sesión.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task RenovarSesionConCuentaDesactivadaRechazaLaSesion()
    {
        Resultado<SesionCreadaDto> sesion = await _servicio.IniciarSesionAsync(new("ana.perez@universidad.edu", "Clave#2026"), CancellationToken.None);
        Usuario usuario = _repositorio.Usuarios[0];
        usuario.Desactivar(FechasPrueba.Inicio);

        Resultado<SesionCreadaDto> renovacion = await _servicio.RenovarSesionAsync(new(sesion.Valor.TokenRenovacion.Valor), CancellationToken.None);

        Assert.Equal("Autenticacion.SesionInvalida", renovacion.Error.Codigo);
    }
}
