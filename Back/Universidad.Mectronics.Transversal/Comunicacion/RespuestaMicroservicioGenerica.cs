using System.Net;

namespace Universidad.Mectronics.Transversal.Comunicacion;

/// <summary>
/// Respuesta de la llamada a otro microservicio con el cuerpo deserializado.
/// </summary>
/// <typeparam name="T">Tipo del cuerpo de la respuesta.</typeparam>
public sealed class RespuestaMicroservicio<T> : RespuestaMicroservicio
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="RespuestaMicroservicio{T}"/>.</summary>
    /// <param name="codigoEstado">Código HTTP de la respuesta.</param>
    /// <param name="contenido">Cuerpo deserializado, o el valor por defecto si no fue exitosa o vino vacía.</param>
    /// <param name="cuerpoError">Cuerpo de la respuesta cuando no fue exitosa, o <see langword="null"/>.</param>
    public RespuestaMicroservicio(HttpStatusCode codigoEstado, T? contenido, string? cuerpoError)
        : base(codigoEstado, cuerpoError)
    {
        Contenido = contenido;
    }

    /// <summary>Cuerpo deserializado de la respuesta exitosa.</summary>
    public T? Contenido { get; }
}
