using System.Data;

namespace Aplicacion.Seguridad.Interfaces;

/// <summary>Confirma los cambios de un caso de uso (patrón Unit of Work).</summary>
public interface IUnidadTrabajo
{
    /// <summary>
    /// Guarda los cambios pendientes en una sola transacción. Lanza <c>ExcepcionDatoDuplicado</c> si se viola una
    /// restricción única.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando los cambios quedan guardados.</returns>
    Task GuardarCambiosAsync(CancellationToken cancelacion);

    /// <summary>
    /// Ejecuta la operación en una transacción explícita: la confirma si termina bien y la revierte si lanza una
    /// excepción o devuelve un resultado fallido. Ante un fallo transitorio se reintenta completa.
    /// </summary>
    /// <typeparam name="T">Tipo que devuelve la operación, normalmente un <c>Resultado</c>.</typeparam>
    /// <param name="operacion">Operación a ejecutar; recibe el token de cancelación y debe llamar a
    /// <see cref="GuardarCambiosAsync"/>.</param>
    /// <param name="nivelAislamiento">Nivel de aislamiento; <see cref="IsolationLevel.Serializable"/> para reglas que
    /// dependen de conteos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Lo que devuelve la operación.</returns>
    Task<T> EjecutarEnTransaccionAsync<T>(
        Func<CancellationToken, Task<T>> operacion,
        IsolationLevel nivelAislamiento,
        CancellationToken cancelacion);
}
