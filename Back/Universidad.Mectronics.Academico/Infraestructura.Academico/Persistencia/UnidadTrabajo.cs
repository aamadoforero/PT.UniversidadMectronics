using System.Data;
using Aplicacion.Academico.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Universidad.Mectronics.Transversal.Excepciones;
using Universidad.Mectronics.Transversal.Resultados;

namespace Infraestructura.Academico.Persistencia;

/// <summary>
/// Implementación de <see cref="IUnidadTrabajo"/> sobre <see cref="ContextoAcademico"/>.
/// </summary>
/// <param name="contexto">Contexto de la petición en curso.</param>
internal sealed class UnidadTrabajo(ContextoAcademico contexto) : IUnidadTrabajo
{
    /// <summary>Error de SQL Server por clave duplicada en un índice único.</summary>
    private const int ErrorIndiceUnicoDuplicado = 2601;

    /// <summary>Error de SQL Server por violación de una restricción <c>UNIQUE</c> o <c>PRIMARY KEY</c>.</summary>
    private const int ErrorRestriccionUnicaDuplicada = 2627;

    /// <summary>
    /// Guarda todos los cambios pendientes y traduce las violaciones de unicidad a <see cref="ExcepcionDatoDuplicado"/>.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Una tarea que termina cuando los cambios quedan guardados.</returns>
    public async Task GuardarCambiosAsync(CancellationToken cancelacion)
    {
        try
        {
            await contexto.SaveChangesAsync(cancelacion);
        }
        catch (DbUpdateException excepcion) when (excepcion.InnerException is SqlException
        {
            Number: ErrorIndiceUnicoDuplicado or ErrorRestriccionUnicaDuplicada
        })
        {
            throw new ExcepcionDatoDuplicado("Ya existe un registro con esos datos. Actualice la información e intente de nuevo.", excepcion);
        }
    }

    /// <summary>
    /// Ejecuta la operación dentro de una transacción explícita, compatible con la estrategia de reintentos.
    /// </summary>
    /// <typeparam name="T">Tipo que devuelve la operación, normalmente un <c>Resultado</c>.</typeparam>
    /// <param name="operacion">Operación completa: lecturas, reglas y escrituras.</param>
    /// <param name="nivelAislamiento">Nivel de aislamiento de la transacción.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Lo que devuelve la operación.</returns>
    public Task<T> EjecutarEnTransaccionAsync<T>(
        Func<CancellationToken, Task<T>> operacion,
        IsolationLevel nivelAislamiento,
        CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(operacion);

        IExecutionStrategy estrategia = contexto.Database.CreateExecutionStrategy();
        return estrategia.ExecuteAsync(async intento =>
        {
            contexto.ChangeTracker.Clear();

            await using IDbContextTransaction transaccion =
                await contexto.Database.BeginTransactionAsync(nivelAislamiento, intento);
            T resultado = await operacion(intento);

            if (resultado is Resultado { EsFallido: true })
            {
                await transaccion.RollbackAsync(intento);
            }
            else
            {
                await transaccion.CommitAsync(intento);
            }

            return resultado;
        }, cancelacion);
    }
}
