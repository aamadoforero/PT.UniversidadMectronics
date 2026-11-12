using Api.Academico.Configuracion;
using Aplicacion.Academico;
using Infraestructura.Academico;

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);
constructor.Services
    .InyectarDependenciasAplicacion()
    .InyectarDependenciasInfraestructura(constructor.Configuration)
    .ConfiguracionApi(constructor.Configuration);

WebApplication aplicacion = constructor.Build();
aplicacion.UsarApiAcademico();
aplicacion.Run();
