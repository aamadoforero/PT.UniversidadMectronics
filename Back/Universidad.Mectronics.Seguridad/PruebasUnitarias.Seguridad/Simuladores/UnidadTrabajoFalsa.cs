using System.Data;
using Aplicacion.Seguridad.Interfaces;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>Unidad de trabajo simulada que cuenta cuántas veces se guardó.</summary>
public sealed class UnidadTrabajoFalsa : IUnidadTrabajo
{
    /// <summary>Cantidad de veces que se llamó a <see cref="GuardarCambiosAsync"/>.</summary>
    public int VecesGuardado { get; private set; }

    /// <summary>Registra el guardado.</summary>
    /// <param name="cancelacion">No se usa.</param>
    /// <returns>Una tarea completada.</returns>
    public Task GuardarCambiosAsync(CancellationToken cancelacion)
    {
        VecesGuardado++;
        return Task.CompletedTask;
    }

    /// <summary>Ejecuta la operación sin transacción real.</summary>
    /// <typeparam name="T">Tipo que devuelve la operación.</typeparam>
    /// <param name="operacion">Operación.</param>
    /// <param name="nivelAislamiento">No se usa.</param>
    /// <param name="cancelacion">Se pasa a la operación.</param>
    /// <returns>Lo que devuelve la operación.</returns>
    public Task<T> EjecutarEnTransaccionAsync<T>(Func<CancellationToken, Task<T>> operacion, IsolationLevel nivelAislamiento, CancellationToken cancelacion) =>
        operacion(cancelacion);
}
