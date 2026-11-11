using Api.Seguridad.Configuracion;
using Aplicacion.Seguridad;
using Infraestructura.Seguridad;

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);
constructor.Services
    .InyectarDependenciasAplicacion()
    .InyectarDependenciasInfraestructura(constructor.Configuration)
    .ConfiguracionApi(constructor.Configuration);

WebApplication aplicacion = constructor.Build();
aplicacion.UsarApiSeguridad();
aplicacion.Run();

/// <summary>
/// Clase del punto de entrada. Se declara pública y parcial para que las pruebas de integración puedan levantar la API
/// con <c>WebApplicationFactory&lt;Program&gt;</c>.
/// </summary>
public partial class Program;
