using Dominio.Seguridad.Autorizacion;
using Dominio.Seguridad.Excepciones;

namespace Dominio.Seguridad.Entidades;

/// <summary>
/// Funcionalidad del sistema que se puede autorizar a un rol (tabla <c>Seguridad.Permiso</c>). Incluye los permisos
/// de todos los microservicios, por eso el código solo se valida por su formato y no contra <see cref="CodigosPermiso"/>.
/// </summary>
public sealed class Permiso
{
    /// <summary>Longitud máxima del código (columna <c>NVARCHAR(100)</c>).</summary>
    public const int LongitudMaximaCodigo = 100;

    /// <summary>Longitud máxima de la descripción (columna <c>NVARCHAR(250)</c>).</summary>
    public const int LongitudMaximaDescripcion = 250;

    /// <summary>Constructor requerido por Entity Framework para materializar el registro.</summary>
    private Permiso()
    {
        Codigo = string.Empty;
        Descripcion = string.Empty;
    }

    /// <summary>
    /// Crea un permiso.
    /// </summary>
    /// <param name="codigo">Código único en minúsculas, números, puntos y guiones (por ejemplo, <c>usuarios.crear</c>).</param>
    /// <param name="descripcion">Explicación de la funcionalidad que habilita.</param>
    public Permiso(string codigo, string descripcion)
    {
        Codigo = ValidarCodigo(codigo);
        Descripcion = ValidarDescripcion(descripcion);
    }

    /// <summary>Identificador generado por la base de datos (PK).</summary>
    public short Id { get; private set; }

    /// <summary>
    /// Código único del permiso (por ejemplo, <c>usuarios.crear</c>). Es el valor que viaja en el token
    /// de acceso y que exigen los endpoints.
    /// </summary>
    public string Codigo { get; private set; }

    /// <summary>Explicación legible de la funcionalidad que habilita el permiso.</summary>
    public string Descripcion { get; private set; }

    /// <summary>Exige minúsculas ASCII, números, puntos y guiones, sin exceder la longitud máxima.</summary>
    private static string ValidarCodigo(string? codigo)
    {
        if (string.IsNullOrEmpty(codigo) ||
            codigo.Length > LongitudMaximaCodigo ||
            !codigo.All(caracter => char.IsAsciiLetterLower(caracter) || char.IsAsciiDigit(caracter) || caracter is '.' or '-'))
        {
            throw new ExcepcionDominio(
                $"El código de permiso '{codigo}' no es válido: use minúsculas, números, puntos y guiones (máximo {LongitudMaximaCodigo}).");
        }

        return codigo;
    }

    /// <summary>Exige una descripción no vacía dentro de la longitud máxima; quita los espacios de los extremos.</summary>
    private static string ValidarDescripcion(string? descripcion)
    {
        string normalizada = descripcion?.Trim() ?? string.Empty;
        if (normalizada.Length == 0 || normalizada.Length > LongitudMaximaDescripcion)
        {
            throw new ExcepcionDominio(
                $"La descripción del permiso es obligatoria (máximo {LongitudMaximaDescripcion} caracteres).");
        }

        return normalizada;
    }
}
