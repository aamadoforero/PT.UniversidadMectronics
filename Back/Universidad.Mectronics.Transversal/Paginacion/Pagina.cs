namespace Universidad.Mectronics.Transversal.Paginacion;

/// <summary>
/// Página de resultados de una consulta paginada: los elementos de la página actual y los datos para navegar entre páginas.
/// La comparten todos los microservicios para responder las consultas paginadas con la misma estructura.
/// </summary>
/// <typeparam name="T">Tipo de los elementos de la página.</typeparam>
public sealed record Pagina<T>
{
    /// <summary>
    /// Crea la página y valida que sus datos sean coherentes.
    /// </summary>
    /// <param name="elementos">Elementos de la página actual.</param>
    /// <param name="numeroPagina">Número de la página actual, empezando en 1.</param>
    /// <param name="tamanoPagina">Cantidad máxima de elementos por página.</param>
    /// <param name="totalElementos">Cantidad total de elementos que cumplen la consulta, en todas las páginas.</param>
    public Pagina(IReadOnlyList<T> elementos, int numeroPagina, int tamanoPagina, int totalElementos)
    {
        ArgumentNullException.ThrowIfNull(elementos);
        ArgumentOutOfRangeException.ThrowIfLessThan(numeroPagina, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(tamanoPagina, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalElementos);

        Elementos = elementos;
        NumeroPagina = numeroPagina;
        TamanoPagina = tamanoPagina;
        TotalElementos = totalElementos;
    }

    /// <summary>Elementos de la página actual.</summary>
    public IReadOnlyList<T> Elementos { get; }

    /// <summary>Número de la página actual, empezando en 1.</summary>
    public int NumeroPagina { get; }

    /// <summary>Cantidad máxima de elementos por página.</summary>
    public int TamanoPagina { get; }

    /// <summary>Cantidad total de elementos que cumplen la consulta, en todas las páginas.</summary>
    public int TotalElementos { get; }

    /// <summary>Cantidad total de páginas; es cero si no hay elementos.</summary>
    public int TotalPaginas => (int)Math.Ceiling(TotalElementos / (double)TamanoPagina);

    /// <summary>
    /// Crea una página con los mismos datos de navegación pero con otro tipo de elemento. Sirve para convertir,
    /// por ejemplo, una página de entidades en una página de DTOs.
    /// </summary>
    /// <typeparam name="TDestino">Tipo de los elementos de la nueva página.</typeparam>
    /// <param name="convertir">Función que convierte cada elemento.</param>
    /// <returns>La página con los elementos convertidos.</returns>
    public Pagina<TDestino> Convertir<TDestino>(Func<T, TDestino> convertir) =>
        new(Elementos.Select(convertir).ToList(), NumeroPagina, TamanoPagina, TotalElementos);
}
