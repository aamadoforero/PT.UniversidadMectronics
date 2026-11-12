using Api.Gateway.Configuracion;

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);
constructor.Services.ConfiguracionGateway(constructor.Configuration);

WebApplication aplicacion = constructor.Build();
aplicacion.UsarGateway();
aplicacion.Run();
