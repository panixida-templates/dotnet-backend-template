using Organization.Product.Host.Common;
using Organization.Product.Module.Infrastructure.DependencyInjection;
using Organization.Product.Module.Presentation.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = FilesConstants.FileRequestSizeLimit;
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation(builder.Configuration);

builder.Host.UseInfrastructure(builder.Configuration);

var app = builder.Build();

app.UsePresentation();

await app.RunAsync();
