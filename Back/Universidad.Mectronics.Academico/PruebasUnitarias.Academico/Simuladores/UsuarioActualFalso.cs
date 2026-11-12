using Aplicacion.Academico.Interfaces;

namespace PruebasUnitarias.Academico.Simuladores;

/// <summary>
/// Cuenta autenticada simulada.
/// </summary>
/// <param name="id">Identificador de la cuenta.</param>
/// <param name="permisos">Permisos de la sesión.</param>
public sealed class UsuarioActualFalso(int id, params string[] permisos) : IUsuarioActual
{
    /// <summary>Siempre autenticado.</summary>
    public bool EstaAutenticado => true;

    /// <summary>Identificador de la cuenta.</summary>
    public int Id { get; } = id;

    /// <summary>
    /// Indica si el permiso está en la lista.
    /// </summary>
    /// <param name="codigo">Código del permiso.</param>
    /// <returns><see langword="true"/> si lo tiene.</returns>
    public bool TienePermiso(string codigo) => permisos.Contains(codigo, StringComparer.Ordinal);
}
