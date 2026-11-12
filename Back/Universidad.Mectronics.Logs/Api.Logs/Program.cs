using Api.Logs.Configuracion;
using Aplicacion.Logs;
using Infraestructura.Logs;

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);
constructor.Services
    .InyectarDependenciasAplicacion()
    .InyectarDependenciasInfraestructura(constructor.Configuration)
    .ConfiguracionApi(constructor.Configuration);

WebApplication aplicacion = constructor.Build();
aplicacion.UsarApiLogs();
aplicacion.Run();
