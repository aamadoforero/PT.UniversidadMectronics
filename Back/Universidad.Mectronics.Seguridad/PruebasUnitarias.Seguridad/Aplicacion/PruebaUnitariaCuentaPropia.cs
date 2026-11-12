using Aplicacion.Seguridad.Dtos.Autenticacion;
using Aplicacion.Seguridad.Servicios.Autenticacion;
using Aplicacion.Seguridad.Servicios.CuentaPropia;
using Aplicacion.Seguridad.Validadores.CuentaPropia;
using Dominio.Seguridad.Entidades;
using PruebasUnitarias.Seguridad.DatosMemoria;
using PruebasUnitarias.Seguridad.Simuladores;
using Universidad.Mectronics.Transversal.Resultados;

namespace PruebasUnitarias.Seguridad.Aplicacion;

/// <summary>
/// Cambio de contraseña de la propia cuenta: exige la contraseña actual, aplica la política y entrega una sesión nueva.
/// </summary>
public sealed class PruebaUnitariaCuentaPropia
{
    /// <summary>Cuenta de la sesión, con contraseña temporal <c>Temporal#2026</c>.</summary>
    private readonly Usuario _usuario = new ConstructorUsuario().AsignarContrasena("Temporal#2026").Construir();

    /// <summary>Unidad de trabajo simulada.</summary>
    private readonly UnidadTrabajoFalsa _unidadTrabajo = new();

    /// <summary>Servicio bajo prueba.</summary>
    private readonly ServicioCuentaPropia _servicio;

    /// <summary>Inicializa una nueva instancia de la clase <see cref="PruebaUnitariaCuentaPropia"/>.</summary>
    public PruebaUnitariaCuentaPropia()
    {
        RepositorioUsuariosEnMemoria repositorio = new RepositorioUsuariosEnMemoria().Sembrar(_usuario);
        _servicio = new ServicioCuentaPropia(
            new UsuarioActualFalso().Autenticar(_usuario), repositorio, _unidadTrabajo, new ServicioHashFalso(), new RelojFijo(),
            new RegistroEventosFalso(), new GeneradorSesion(new ServicioTokensFalso()),
            new ValidadorCambiarContrasena());
    }

    /// <summary>
    /// Si la contraseña actual no coincide, responde un error del campo y no cambia nada.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarContrasenaConContrasenaActualIncorrectaDevuelveErrorDelCampo()
    {
        Resultado<SesionCreadaDto> resultado = await _servicio.CambiarContrasenaAsync(new("Otra#2026", "Nueva#2026x"), CancellationToken.None);

        Assert.Equal("CuentaPropia.ContrasenaActualIncorrecta", resultado.Error.Codigo);
        Assert.Contains("ContrasenaActual", resultado.Error.Detalles!.Keys);
        Assert.True(_usuario.DebeCambiarContrasena);
        Assert.Equal(0, _unidadTrabajo.VecesGuardado);
    }

    /// <summary>
    /// Una contraseña nueva débil responde todos los requisitos incumplidos en el campo correspondiente.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarContrasenaConContrasenaDebilListaTodosLosRequisitos()
    {
        Resultado<SesionCreadaDto> resultado = await _servicio.CambiarContrasenaAsync(new("Temporal#2026", "abc"), CancellationToken.None);

        Assert.Equal(TipoError.Validacion, resultado.Error.Tipo);
        Assert.Equal(4, resultado.Error.Detalles!["ContrasenaNueva"].Length);
    }

    /// <summary>
    /// Con datos válidos guarda el hash nuevo, deja de ser temporal y entrega una sesión nueva.
    /// </summary>
    /// <returns>Una tarea que termina con la verificación.</returns>
    [Fact]
    public async Task CambiarContrasenaConDatosValidosGuardaElHashYEntregaSesionNueva()
    {
        Resultado<SesionCreadaDto> resultado = await _servicio.CambiarContrasenaAsync(new("Temporal#2026", "Nueva#2026x"), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.False(resultado.Valor.Usuario.DebeCambiarContrasena);
        Assert.Equal("hash:Nueva#2026x", _usuario.Contrasena);
        Assert.Equal(1, _unidadTrabajo.VecesGuardado);
    }
}
