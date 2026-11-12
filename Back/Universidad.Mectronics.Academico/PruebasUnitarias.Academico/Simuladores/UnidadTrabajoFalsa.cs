using System.Data;
using Aplicacion.Academico.Interfaces;

namespace PruebasUnitarias.Academico.Simuladores;

/// <summary>
/// Unidad de trabajo simulada: cuenta los guardados y registra el nivel de aislamiento de la última transacción.
/// </summary>
public sealed class UnidadTrabajoFalsa : IUnidadTrabajo
{
    /// <summary>Número de veces que se guardaron cambios.</summary>
    public int VecesGuardado { get; private set; }

    /// <summary>Nivel de aislamiento de la última transacción, o <see langword="null"/> si no hubo ninguna.</summary>
    public IsolationLevel? UltimoNivelAislamiento { get; private set; }

    /// <summary>
    /// Registra el guardado.
    /// </summary>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Una tarea completada.</returns>
    public Task GuardarCambiosAsync(CancellationToken cancelacion)
    {
        VecesGuardado++;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Registra el nivel de aislamiento y ejecuta la operación directamente.
    /// </summary>
    /// <typeparam name="T">Tipo del resultado.</typeparam>
    /// <param name="operacion">Operación a ejecutar.</param>
    /// <param name="nivelAislamiento">Nivel de aislamiento pedido.</param>
    /// <param name="cancelacion">Cancelación que recibe la operación.</param>
    /// <returns>El resultado de la operación.</returns>
    public Task<T> EjecutarEnTransaccionAsync<T>(Func<CancellationToken, Task<T>> operacion, IsolationLevel nivelAislamiento, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(operacion);
        UltimoNivelAislamiento = nivelAislamiento;
        return operacion(cancelacion);
    }
}
