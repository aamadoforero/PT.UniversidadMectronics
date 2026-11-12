using System.Data;

namespace Aplicacion.Academico.Interfaces;

/// <summary>
/// Confirma los cambios de los repositorios y ejecuta operaciones dentro de una transacción.
/// </summary>
public interface IUnidadTrabajo
{
    /// <summary>
    /// Guarda los cambios pendientes.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando los cambios quedan guardados.</returns>
    Task GuardarCambiosAsync(CancellationToken cancelacion);

    /// <summary>
    /// Ejecuta la operación en una transacción: la confirma si el resultado es exitoso y la revierte si falla. Ante un
    /// fallo transitorio se reintenta completa.
    /// </summary>
    /// <typeparam name="T">Tipo del resultado de la operación.</typeparam>
    /// <param name="operacion">Operación a ejecutar; recibe la cancelación de cada intento.</param>
    /// <param name="nivelAislamiento">Nivel de aislamiento de la transacción.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El resultado de la operación.</returns>
    Task<T> EjecutarEnTransaccionAsync<T>(
        Func<CancellationToken, Task<T>> operacion,
        IsolationLevel nivelAislamiento,
        CancellationToken cancelacion);
}
