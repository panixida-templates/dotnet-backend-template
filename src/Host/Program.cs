using Application.Users.Create;

using Infrastructure.Mediator.Wolverine.DependencyInjection;
using Infrastructure.Persistence.Ef.DependencyInjection;

using Logging.OpenSearch;

using Presentation.Host.Common;
using Presentation.Http.Bootstrapper;
using Presentation.Http.Common;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.ConfigureGrpcClients(builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = FilesConstants.FileRequestSizeLimit;
});

builder.Services.AddEfRepository(builder.Configuration);
builder.Services.AddHttp();
builder.Services.AddWolverineMediator();
builder.Host.UseWolverineMediator(builder.Configuration, typeof(CreateUserHandler).Assembly);

//builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

if (!builder.Environment.IsEnvironment(EnvironmentConstants.Test))
{
    //builder.Services.AddPrometheusMetrics();
    builder.Host.UseSerilog(OpenSearchConfiguration.ConfigureOpenSearch(builder.Configuration));
}

var app = builder.Build();

app.MapHttp();

app.UseHttpsRedirection();

if (!builder.Environment.IsEnvironment(EnvironmentConstants.Test))
{
    //app.UsePrometheusMetrics();
}

//app.UseAuthenticationAndAuthorization();

await app.RunAsync();