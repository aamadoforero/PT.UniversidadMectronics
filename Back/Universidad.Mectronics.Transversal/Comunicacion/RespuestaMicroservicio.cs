using System.Net;

namespace Universidad.Mectronics.Transversal.Comunicacion;

/// <summary>
/// Respuesta de la llamada a otro microservicio.
/// </summary>
public class RespuestaMicroservicio
{
    /// <summary>Inicializa una nueva instancia de la clase <see cref="RespuestaMicroservicio"/>.</summary>
    /// <param name="codigoEstado">Código HTTP de la respuesta.</param>
    /// <param name="cuerpoError">Cuerpo de la respuesta cuando no fue exitosa, o <see langword="null"/>.</param>
    public RespuestaMicroservicio(HttpStatusCode codigoEstado, string? cuerpoError)
    {
        CodigoEstado = codigoEstado;
        CuerpoError = cuerpoError;
    }

    /// <summary>Código HTTP de la respuesta.</summary>
    public HttpStatusCode CodigoEstado { get; }

    /// <summary>Indica si el código HTTP es 2xx.</summary>
    public bool EsExitosa => (int)CodigoEstado is >= 200 and <= 299;

    /// <summary>Cuerpo de la respuesta cuando no fue exitosa (por lo general un Problem Details).</summary>
    public string? CuerpoError { get; }
}
