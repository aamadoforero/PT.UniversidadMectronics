using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Universidad.Mectronics.Transversal.Auditoria;
using Universidad.Mectronics.Transversal.Configuracion;
using Universidad.Mectronics.Transversal.Resiliencia;

namespace Universidad.Mectronics.Transversal.Comunicacion;

/// <summary>
/// Registro de la comunicación entre microservicios y de la auditoría, que viaja por ella al microservicio de Logs.
/// </summary>
public static class RegistroDependenciasComunicacion
{
    /// <summary>
    /// Registra <see cref="IClienteMicroservicios"/> con la política de resiliencia y <see cref="IRegistroEventos"/>. Falla al
    /// arrancar si falta la URL de algún microservicio, la de Logs o la clave de auditoría.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <param name="configuracion">Configuración con la sección <c>Microservicios</c> y la clave <c>Logs:ClaveAplicacion</c>.</param>
    /// <param name="opciones">Parámetros de la resiliencia, o <see langword="null"/> para usar los valores por defecto.</param>
    /// <returns>La misma colección, para encadenar registros.</returns>
    public static IServiceCollection InyectarClienteMicroservicios(
        this IServiceCollection servicios, IConfiguration configuracion, OpcionesResiliencia? opciones = null)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        DireccionesMicroservicios direcciones = LeerDirecciones(configuracion);
        string claveAuditoria = LeerClaveAuditoria(configuracion);

        servicios.AddSingleton(direcciones);
        servicios.AddHttpClient(ClienteMicroservicios.NombreCliente).AgregarResiliencia(opciones ?? new OpcionesResiliencia());
        servicios.AddSingleton<IClienteMicroservicios, ClienteMicroservicios>();

        servicios.TryAddSingleton(TimeProvider.System);
        servicios.AddSingleton<IRegistroEventos>(proveedor => new RegistroEventosMicroservicio(
            proveedor.GetRequiredService<IClienteMicroservicios>(),
            claveAuditoria,
            proveedor.GetRequiredService<TimeProvider>(),
            proveedor.GetRequiredService<ILogger<RegistroEventosMicroservicio>>()));

        return servicios;
    }

    /// <summary>
    /// Lee y valida las URL de la sección <c>Microservicios</c>; la de Logs es obligatoria.
    /// </summary>
    /// <param name="configuracion">Configuración de la aplicación.</param>
    /// <returns>Las direcciones por nombre de microservicio.</returns>
    private static DireccionesMicroservicios LeerDirecciones(IConfiguration configuracion)
    {
        Dictionary<string, Uri> direcciones = new(StringComparer.OrdinalIgnoreCase);

        foreach (IConfigurationSection seccion in configuracion.GetSection(DireccionesMicroservicios.Seccion).GetChildren())
        {
            string clave = $"{DireccionesMicroservicios.Seccion}:{seccion.Key}";
            if (!Uri.TryCreate(seccion.Value, UriKind.Absolute, out Uri? url))
            {
                throw new InvalidOperationException(
                    MensajesConfiguracion.CrearMensajeInvalido(clave, "debe ser una URL absoluta, por ejemplo http://localhost:5092."));
            }

            direcciones[seccion.Key] = url.AbsoluteUri.EndsWith('/') ? url : new Uri(url.AbsoluteUri + "/");
        }

        if (!direcciones.ContainsKey(RegistroEventosMicroservicio.NombreServicio))
        {
            throw new InvalidOperationException(MensajesConfiguracion.CrearMensajeFaltante(
                $"{DireccionesMicroservicios.Seccion}:{RegistroEventosMicroservicio.NombreServicio}"));
        }

        return new DireccionesMicroservicios(direcciones);
    }

    /// <summary>
    /// Lee la clave con la que este microservicio se identifica ante el de Logs.
    /// </summary>
    /// <param name="configuracion">Configuración de la aplicación.</param>
    /// <returns>La clave.</returns>
    private static string LeerClaveAuditoria(IConfiguration configuracion)
    {
        string? clave = configuracion[RegistroEventosMicroservicio.ClaveConfiguracion];
        if (string.IsNullOrWhiteSpace(clave))
        {
            throw new InvalidOperationException(
                MensajesConfiguracion.CrearMensajeFaltante(RegistroEventosMicroservicio.ClaveConfiguracion));
        }

        return clave;
    }
}
