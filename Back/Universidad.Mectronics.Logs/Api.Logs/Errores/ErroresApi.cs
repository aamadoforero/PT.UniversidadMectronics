using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Logs.Errores;

/// <summary>
/// Errores que produce la propia API antes de llegar a un caso de uso.
/// </summary>
public static class ErroresApi
{
    /// <summary>
    /// Crea el error para una petición sin la cabecera de la clave o con una clave que no corresponde a ninguna aplicación.
    /// </summary>
    /// <returns>El error de tipo no autorizado (401).</returns>
    public static Error CrearClaveInvalida() => Error.CrearNoAutorizado(
        "Autenticacion.ClaveAplicacionInvalida",
        "Falta la cabecera X-Api-Key o su valor no corresponde a ninguna aplicación.");
}
