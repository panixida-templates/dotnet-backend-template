using JasperFx;

using Organization.Product.Host.Common;
using Organization.Product.Host.Configurations;
using Organization.Product.Host.Configurations.Modules;
using Organization.Product.Module.Presentation.DependencyInjection;

using PANiXiDA.Core.Presentation.Scalar.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability();

builder.WebHost.ConfigureKestrel(
    options =>
    {
        options.Limits.MaxRequestBodySize = FilesConstants.FileRequestSizeLimit;
    });

builder.AddModuleModule();
builder.AddMessaging();
builder.Services.AddApiDocumentation(builder.Configuration);

var app = builder.Build();

app.UsePresentation();
app.UseApiDocumentation();

return await app.RunJasperFxCommands(args);
