using Aplicacion.Seguridad.Dtos.Usuarios;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Errores;

/// <summary>
/// Errores esperados de la administración de cuentas.
/// </summary>
public static class ErroresUsuario
{
    /// <summary>
    /// Crea el error para datos faltantes o con formato inválido.
    /// </summary>
    /// <param name="detalles">Campos inválidos y sus mensajes.</param>
    /// <returns>El error de validación.</returns>
    public static Error CrearDatosInvalidos(IReadOnlyDictionary<string, string[]> detalles) => Error.CrearValidacion(
        "Usuario.DatosInvalidos",
        "Revise los datos ingresados.",
        detalles);

    /// <summary>
    /// Crea el error para una cuenta inexistente.
    /// </summary>
    /// <param name="id">Identificador buscado.</param>
    /// <returns>El error de recurso inexistente.</returns>
    public static Error CrearNoEncontrado(int id) => Error.CrearNoEncontrado(
        "Usuario.NoEncontrado",
        $"No existe una cuenta con el identificador {id}.");

    /// <summary>
    /// Crea el error para un correo que ya usa otra cuenta.
    /// </summary>
    /// <returns>El error de conflicto, con el detalle del campo.</returns>
    public static Error CrearEmailDuplicado() => new(
        "Usuario.EmailDuplicado",
        "Ya existe una cuenta con ese correo.",
        TipoError.Conflicto,
        new Dictionary<string, string[]> { [nameof(CrearUsuarioDto.Email)] = ["Ya existe una cuenta con ese correo."] });

    /// <summary>
    /// Crea el error para una identificación que ya usa otra cuenta.
    /// </summary>
    /// <returns>El error de conflicto, con el detalle del campo.</returns>
    public static Error CrearIdentificacionDuplicada() => new(
        "Usuario.IdentificacionDuplicada",
        "Ya existe una cuenta con esa identificación.",
        TipoError.Conflicto,
        new Dictionary<string, string[]> { [nameof(CrearUsuarioDto.Identificacion)] = ["Ya existe una cuenta con esa identificación."] });

    /// <summary>
    /// Crea el error para un rol inexistente.
    /// </summary>
    /// <returns>El error de validación del campo del rol.</returns>
    public static Error CrearRolNoEncontrado() => Error.CrearValidacion(
        "Usuario.RolNoEncontrado",
        "El rol seleccionado no existe.",
        new Dictionary<string, string[]> { [nameof(CrearUsuarioDto.RolId)] = ["El rol seleccionado no existe."] });

    /// <summary>
    /// Crea el error para un actor sin el permiso que exige la operación.
    /// </summary>
    /// <returns>El error de falta de permiso.</returns>
    public static Error CrearSinPermiso() => Error.CrearProhibido(
        "Usuario.SinPermiso",
        "No tiene permiso para realizar esta operación.");

    /// <summary>
    /// Crea el error para una operación que el actor intenta sobre su propia cuenta y que no está permitida.
    /// </summary>
    /// <param name="descripcion">Explicación de qué no puede hacer y qué alternativa tiene.</param>
    /// <returns>El error de conflicto.</returns>
    public static Error CrearOperacionSobreCuentaPropia(string descripcion) => Error.CrearConflicto(
        "Usuario.OperacionSobreCuentaPropia",
        descripcion);
}
