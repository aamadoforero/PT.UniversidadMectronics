using Aplicacion.Seguridad.Dtos.CuentaPropia;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Errores;

/// <summary>
/// Errores esperados de las operaciones que el usuario hace sobre su propia cuenta.
/// </summary>
public static class ErroresCuentaPropia
{
    /// <summary>
    /// Crea el error para datos faltantes o con formato inválido.
    /// </summary>
    /// <param name="detalles">Campos inválidos y sus mensajes.</param>
    /// <returns>El error de validación.</returns>
    public static Error CrearDatosInvalidos(IReadOnlyDictionary<string, string[]> detalles) => Error.CrearValidacion(
        "CuentaPropia.DatosInvalidos",
        "Revise los datos ingresados.",
        detalles);

    /// <summary>
    /// Crea el error para una contraseña actual que no coincide, asociado al campo.
    /// </summary>
    /// <returns>El error de validación.</returns>
    public static Error CrearContrasenaActualIncorrecta() => Error.CrearValidacion(
        "CuentaPropia.ContrasenaActualIncorrecta",
        "La contraseña actual no es correcta.",
        new Dictionary<string, string[]>
        {
            [nameof(CambiarContrasenaDto.ContrasenaActual)] = ["La contraseña actual no es correcta."]
        });
}
