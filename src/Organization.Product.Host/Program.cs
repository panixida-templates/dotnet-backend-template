using JasperFx;

using Organization.Product.Host.Configurations;
using Organization.Product.Host.Configurations.Modules;

using PANiXiDA.Core.Presentation.Scalar.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability();
builder.AddHttp();
builder.AddModuleModule();
builder.AddMessaging();
builder.Services.AddApiDocumentation(builder.Configuration);

var app = builder.Build();

app.UseHttp();
app.UseApiDocumentation();

return await app.RunJasperFxCommands(args);
