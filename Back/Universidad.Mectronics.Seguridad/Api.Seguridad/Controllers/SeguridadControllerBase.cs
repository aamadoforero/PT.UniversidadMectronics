using Api.Seguridad.Errores;
using Microsoft.AspNetCore.Mvc;
using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Seguridad.Controllers;

/// <summary>
/// Base de los controladores del servicio. Traduce los <see cref="Resultado"/> de la capa de Aplicación a respuestas HTTP con
/// el mismo formato Problem Details que usan los middlewares, de modo que los controladores solo orquestan HTTP y no
/// contienen lógica de negocio.
/// </summary>
[ApiController]
public abstract class SeguridadControllerBase : ControllerBase
{
    /// <summary>
    /// Convierte un resultado con datos en 200 con el valor, o en el problema correspondiente.
    /// </summary>
    /// <typeparam name="T">Tipo de los datos del resultado.</typeparam>
    /// <param name="resultado">Resultado del caso de uso.</param>
    /// <returns>La respuesta HTTP.</returns>
    protected IActionResult ConvertirARespuesta<T>(Resultado<T> resultado)
    {
        ArgumentNullException.ThrowIfNull(resultado);
        return resultado.EsExitoso ? Ok(resultado.Valor) : ConvertirAProblema(resultado.Error);
    }

    /// <summary>
    /// Convierte un error en una respuesta Problem Details: título para el usuario, extensión <c>codigo</c> para el frontend y
    /// extensión <c>errores</c> con el detalle por campo cuando lo hay. El código HTTP sale de <see cref="TipoError"/>.
    /// </summary>
    /// <param name="error">Error esperado.</param>
    /// <returns>La respuesta HTTP con el problema.</returns>
    protected ObjectResult ConvertirAProblema(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        ObjectResult respuesta = Problem(title: error.Descripcion, statusCode: ExtensionesResultadoHttp.ObtenerCodigoEstado(error.Tipo));
        ProblemDetails problema = (ProblemDetails)respuesta.Value!;
        problema.Extensions["codigo"] = error.Codigo;
        if (error.Detalles is not null)
        {
            problema.Extensions["errores"] = error.Detalles;
        }

        return respuesta;
    }
}
