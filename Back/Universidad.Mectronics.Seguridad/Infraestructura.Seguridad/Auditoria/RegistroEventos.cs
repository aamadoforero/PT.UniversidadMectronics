using Aplicacion.Seguridad.Interfaces;
using Infraestructura.Seguridad.Configuracion;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Infraestructura.Seguridad.Auditoria;

/// <summary>
/// Implementación de <see cref="IRegistroEventos"/> que escribe en <c>Logs.RegistroEvento</c> con la aplicación
/// <c>Servicio-Seguridad</c>. Usa su propia conexión, fuera de Entity Framework, por dos razones: el evento queda guardado
/// aunque la transacción del caso de uso se revierta, y un fallo al registrar nunca interrumpe la operación principal
/// (solo se informa en el log de la aplicación). Ignora la cancelación de la petición para no perder eventos de auditoría.
/// </summary>
/// <param name="opcionesBaseDatos">Cadena de conexión.</param>
/// <param name="registrador">Log de la aplicación, donde también se escribe cada evento.</param>
internal sealed class RegistroEventos(OpcionesBaseDatos opcionesBaseDatos, ILogger<RegistroEventos> registrador) : IRegistroEventos
{
    /// <summary>Nombre del microservicio en el catálogo <c>Logs.Aplicacion</c>.</summary>
    private const string NombreAplicacion = "Servicio-Seguridad";

    /// <summary>
    /// Inserción parametrizada; los identificadores se resuelven por nombre en los catálogos de <c>Logs</c>.
    /// </summary>
    private const string Insercion = """
        INSERT INTO Logs.RegistroEvento (TipoEventoId, AplicacionId, Mensaje, Excepcion)
        SELECT tipo.Id, aplicacion.Id, @Mensaje, @Excepcion
        FROM Logs.TipoEvento tipo
        CROSS JOIN Logs.Aplicacion aplicacion
        WHERE tipo.Nombre = @TipoEvento AND aplicacion.Nombre = @Aplicacion;
        """;

    /// <summary>
    /// Registra un evento informativo.
    /// </summary>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="cancelacion">No se usa: el evento se registra aunque la petición se cancele.</param>
    /// <returns>Una tarea que termina cuando el evento queda registrado o falla de forma controlada.</returns>
    public Task RegistrarInformacionAsync(string mensaje, CancellationToken cancelacion)
    {
        registrador.LogInformation("{Mensaje}", mensaje);
        return InsertarAsync("Informativo", mensaje, null);
    }

    /// <summary>
    /// Registra una advertencia.
    /// </summary>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="cancelacion">No se usa: el evento se registra aunque la petición se cancele.</param>
    /// <returns>Una tarea que termina cuando el evento queda registrado o falla de forma controlada.</returns>
    public Task RegistrarAdvertenciaAsync(string mensaje, CancellationToken cancelacion)
    {
        registrador.LogWarning("{Mensaje}", mensaje);
        return InsertarAsync("Advertencia", mensaje, null);
    }

    /// <summary>
    /// Registra un error con el detalle técnico de la excepción.
    /// </summary>
    /// <param name="mensaje">Descripción del error.</param>
    /// <param name="excepcion">Excepción que lo causó.</param>
    /// <param name="cancelacion">No se usa: el error se registra aunque la petición se cancele.</param>
    /// <returns>Una tarea que termina cuando el error queda registrado o falla de forma controlada.</returns>
    public Task RegistrarErrorAsync(string mensaje, Exception excepcion, CancellationToken cancelacion)
    {
        registrador.LogError(excepcion, "{Mensaje}", mensaje);
        return InsertarAsync("Error", mensaje, excepcion?.ToString());
    }

    /// <summary>
    /// Inserta el evento en la base de datos. Si falla, lo informa en el log de la aplicación y no lanza la excepción.
    /// </summary>
    /// <param name="tipoEvento">Nombre del tipo en <c>Logs.TipoEvento</c>.</param>
    /// <param name="mensaje">Descripción del evento.</param>
    /// <param name="detalleExcepcion">Detalle técnico, o <see langword="null"/> si no aplica.</param>
    /// <returns>Una tarea que termina cuando el intento de inserción concluye.</returns>
    private async Task InsertarAsync(string tipoEvento, string mensaje, string? detalleExcepcion)
    {
        try
        {
            await using SqlConnection conexion = new(opcionesBaseDatos.CadenaConexion);
            await conexion.OpenAsync(CancellationToken.None);
            await using SqlCommand comando = new(Insercion, conexion);
            comando.Parameters.AddWithValue("@TipoEvento", tipoEvento);
            comando.Parameters.AddWithValue("@Aplicacion", NombreAplicacion);
            comando.Parameters.AddWithValue("@Mensaje", mensaje);
            comando.Parameters.AddWithValue("@Excepcion", (object?)detalleExcepcion ?? DBNull.Value);
            await comando.ExecuteNonQueryAsync(CancellationToken.None);
        }
        catch (Exception excepcion) when (excepcion is SqlException or InvalidOperationException)
        {
            registrador.LogError(excepcion, "No se pudo registrar el evento de auditoría en Logs.RegistroEvento.");
        }
    }
}
