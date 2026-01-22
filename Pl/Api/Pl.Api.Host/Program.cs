using Api.Extensions.Configurations;

using Bl.DependencyInjection;

using Common.Constants;

using Dal.DependencyInjection;

using Logging.OpenSearch;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Pl.Api.Host.Extensions.Configurations;
using Pl.Api.Http.Composition;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureGrpcClients(builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1_073_741_824;
    // если долгие загрузки по медленному каналу:
    // options.Limits.MinRequestBodyDataRate = null;
});

builder.Services.UseBl();
builder.Services.UseDal(builder.Configuration);
builder.Services.AddHttp();

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddSwagger(AppsettingsKeysConstants.ServiceName);

if (!builder.Environment.IsEnvironment(EnvironmentConstants.Test))
{
    //builder.Services.AddPrometheusMetrics();
    builder.Host.UseSerilog(OpenSearchConfiguration.ConfigureOpenSearch(builder.Configuration));
}

var app = builder.Build();

app.UseHttp();

app.UseHttpsRedirection();

if (!builder.Environment.IsEnvironment(EnvironmentConstants.Test))
{
    //app.UsePrometheusMetrics();
}

app.UseSwaggerAndSwaggerUI();
//app.UseAuthenticationAndAuthorization();

await app.RunAsync();