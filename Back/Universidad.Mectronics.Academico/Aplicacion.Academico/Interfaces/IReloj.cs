namespace Aplicacion.Academico.Interfaces;

/// <summary>
/// Fuente de la fecha y hora actual. Permite que las pruebas controlen el tiempo.
/// </summary>
public interface IReloj
{
    /// <summary>Fecha y hora actual en UTC.</summary>
    DateTimeOffset AhoraUtc { get; }
}
