namespace Universidad.Mectronics.Transversal.Resultados;

/// <summary>
/// Categoría de un error esperado. Determina el código HTTP con que la API lo responde, de modo que la
/// correspondencia entre errores y códigos de estado vive en un solo lugar.
/// </summary>
public enum TipoError
{
    /// <summary>Error inesperado de negocio o de la operación. Se responde con HTTP 500.</summary>
    Falla = 0,

    /// <summary>Los datos de entrada no son válidos. Se responde con HTTP 400 e incluye el detalle por campo.</summary>
    Validacion = 1,

    /// <summary>No se pudo identificar quien hace la petición (credenciales o sesión inválidas). Se responde con HTTP 401.</summary>
    NoAutorizado = 2,

    /// <summary>Se sabe quién hace la petición, pero no tiene permiso para la operación. Se responde con HTTP 403.</summary>
    Prohibido = 3,

    /// <summary>El recurso solicitado no existe. Se responde con HTTP 404.</summary>
    NoEncontrado = 4,

    /// <summary>La operación genera conflicto con el estado actual de los datos (por ejemplo, un correo duplicado). Se responde con HTTP 409.</summary>
    Conflicto = 5
}
