using Application.Users.Create;

using Common.Constants;

using Infrastructure.Mediator.Wolverine.DependencyInjection;
using Infrastructure.Persistence.Ef.DependencyInjection;

using Logging.OpenSearch;

using Presentation.Http.Bootstrapper;

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
builder.Host.UseWolverineMediator(typeof(CreateUserHandler).Assembly);

//builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
//builder.Services.AddSwagger(AppsettingsKeysConstants.ServiceName);

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

//app.UseSwaggerAndSwaggerUI();
//app.UseAuthenticationAndAuthorization();

await app.RunAsync();