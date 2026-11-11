using System.Data;

namespace Aplicacion.Seguridad.Interfaces;

/// <summary>
/// Confirma en la base de datos todos los cambios hechos a las entidades durante un caso de uso (patrón Unit of Work).
/// Lo implementa Infraestructura con el contexto de Entity Framework, que es uno por petición y lo comparten los repositorios.
/// </summary>
public interface IUnidadTrabajo
{
    /// <summary>
    /// Guarda todos los cambios pendientes en una sola transacción implícita: se guardan todos o ninguno. Si la base de datos
    /// rechaza un dato duplicado (por ejemplo, dos altas simultáneas con el mismo correo), lanza <c>ExcepcionDatoDuplicado</c>.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando los cambios quedan guardados.</returns>
    Task GuardarCambiosAsync(CancellationToken cancelacion);

    /// <summary>
    /// Ejecuta una operación completa (lecturas, reglas y escrituras) dentro de una transacción explícita. Se usa cuando una
    /// regla depende de un conteo que otra petición simultánea podría cambiar (write skew), por ejemplo "un estudiante inscribe
    /// máximo 3 materias". La transacción se confirma si la operación termina bien y se revierte si lanza una
    /// excepción o devuelve un <c>Resultado</c> fallido. Es compatible con la estrategia de reintentos: ante un fallo
    /// transitorio, la operación completa se vuelve a ejecutar desde cero.
    /// </summary>
    /// <typeparam name="T">Tipo que devuelve la operación, normalmente un <c>Resultado</c>.</typeparam>
    /// <param name="operacion">Operación a ejecutar; recibe el token de cancelación y debe llamar a <see cref="GuardarCambiosAsync"/>.</param>
    /// <param name="nivelAislamiento">Nivel de aislamiento; <see cref="IsolationLevel.Serializable"/> para reglas que dependen de conteos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Lo que devuelve la operación.</returns>
    Task<T> EjecutarEnTransaccionAsync<T>(
        Func<CancellationToken, Task<T>> operacion,
        IsolationLevel nivelAislamiento,
        CancellationToken cancelacion);
}
