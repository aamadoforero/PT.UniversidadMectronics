using Api.Academico.Errores;
using Microsoft.AspNetCore.Mvc;
using Universidad.Mectronics.Transversal.Resultados;

namespace Api.Academico.Controllers;

/// <summary>
/// Controlador base que convierte un <see cref="Resultado"/> en la respuesta HTTP.
/// </summary>
[ApiController]
public abstract class AcademicoControllerBase : ControllerBase
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
    /// Convierte un error en una respuesta Problem Details con el código del error y el detalle por campo.
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
