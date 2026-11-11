using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.ObjetosValor;
using FluentValidation;

namespace Aplicacion.Seguridad.Validadores;

/// <summary>
/// Reglas de FluentValidation que se repiten en varios DTOs. Cada una reutiliza el objeto de valor del dominio, así la regla
/// de formato se define una sola vez y los mensajes son iguales en todos los formularios.
/// </summary>
public static class ExtensionesReglasValidacion
{
    /// <summary>
    /// Exige un nombre completo con contenido y dentro de la longitud máxima.
    /// </summary>
    /// <typeparam name="T">Tipo del DTO que se valida.</typeparam>
    /// <param name="regla">Regla del campo del nombre.</param>
    /// <returns>La regla configurada, para encadenar más condiciones.</returns>
    public static IRuleBuilderOptions<T, string?> ValidarNombreCompleto<T>(this IRuleBuilderInitial<T, string?> regla) =>
        regla.Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .Must(nombre => NombreCompleto.TryCrear(nombre, out _))
            .WithMessage($"El nombre completo admite máximo {NombreCompleto.LongitudMaxima} caracteres.");

    /// <summary>
    /// Exige una identificación con letras, números o guiones y dentro de la longitud máxima.
    /// </summary>
    /// <typeparam name="T">Tipo del DTO que se valida.</typeparam>
    /// <param name="regla">Regla del campo de la identificación.</param>
    /// <returns>La regla configurada, para encadenar más condiciones.</returns>
    public static IRuleBuilderOptions<T, string?> ValidarIdentificacion<T>(this IRuleBuilderInitial<T, string?> regla) =>
        regla.Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("La identificación es obligatoria.")
            .Must(identificacion => Identificacion.TryCrear(identificacion, out _))
            .WithMessage($"La identificación admite letras, números y guiones, y máximo {Identificacion.LongitudMaxima} caracteres.");

    /// <summary>
    /// Exige un correo con formato válido y dentro de la longitud máxima.
    /// </summary>
    /// <typeparam name="T">Tipo del DTO que se valida.</typeparam>
    /// <param name="regla">Regla del campo del correo.</param>
    /// <returns>La regla configurada, para encadenar más condiciones.</returns>
    public static IRuleBuilderOptions<T, string?> ValidarCorreo<T>(this IRuleBuilderInitial<T, string?> regla) =>
        regla.Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .Must(email => CorreoElectronico.TryCrear(email, out _))
            .WithMessage("El correo no tiene un formato válido.");

    /// <summary>
    /// Si se envía una ubicación semestral, exige que esté en el rango permitido. Que sea obligatoria depende del rol y lo
    /// verifica el servicio.
    /// </summary>
    /// <typeparam name="T">Tipo del DTO que se valida.</typeparam>
    /// <param name="regla">Regla del campo de la ubicación semestral.</param>
    /// <returns>La regla configurada, para encadenar más condiciones.</returns>
    public static IRuleBuilderOptions<T, int?> ValidarUbicacionSemestral<T>(this IRuleBuilderInitial<T, int?> regla) =>
        regla.InclusiveBetween(PerfilEstudiante.UbicacionSemestralMinima, PerfilEstudiante.UbicacionSemestralMaxima)
            .WithMessage($"La ubicación semestral debe estar entre {PerfilEstudiante.UbicacionSemestralMinima} y {PerfilEstudiante.UbicacionSemestralMaxima}.");
}
