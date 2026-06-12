using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PANiXiDA.Core.Presentation.Http.DependencyInjection;

using System.Reflection;

namespace Organization.Product.Module.Presentation.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddHttp(configuration);

        return serviceCollection;
    }

    public static WebApplication UsePresentation(
        this WebApplication app,
        params Assembly[] assemblies)
    {
        app.UseHttp(assemblies);

        return app;
    }
}
