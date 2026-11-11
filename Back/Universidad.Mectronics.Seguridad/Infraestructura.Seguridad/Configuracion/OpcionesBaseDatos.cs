namespace Infraestructura.Seguridad.Configuracion;

/// <summary>
/// Cadena de conexión ya validada. La usan los componentes que se conectan a SQL Server sin pasar por Entity Framework,
/// como el registro de eventos.
/// </summary>
/// <param name="CadenaConexion">Cadena de conexión a la base de datos <c>UniversidadMectronics</c>.</param>
public sealed record OpcionesBaseDatos(string CadenaConexion);
