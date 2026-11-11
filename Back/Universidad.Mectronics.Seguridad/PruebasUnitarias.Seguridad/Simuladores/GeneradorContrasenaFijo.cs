using Aplicacion.Seguridad.Interfaces;

namespace PruebasUnitarias.Seguridad.Simuladores;

/// <summary>
/// Generador de contraseñas temporales con un valor conocido, para que la prueba pueda verificar qué se entregó y guardó.
/// </summary>
public sealed class GeneradorContrasenaFijo : IGeneradorContrasenaTemporal
{
    /// <summary>Contraseña temporal que siempre se genera.</summary>
    public const string Contrasena = "Temporal#2026";

    /// <summary>Devuelve la contraseña fija.</summary>
    /// <returns><see cref="Contrasena"/>.</returns>
    public string Generar() => Contrasena;
}
