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
