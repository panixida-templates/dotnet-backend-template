using System.Reflection;

using Application.Abstractions.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wolverine;

namespace Infrastructure.Mediator.Wolverine.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWolverineMediator(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IMediator, WolverineMediator>();

        return services;
    }

    public static IHostBuilder UseWolverineMediator(this IHostBuilder hostBuilder, params Assembly[] discoveryAssemblies)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        hostBuilder.UseWolverine(options =>
        {
            options.Durability.Mode = DurabilityMode.MediatorOnly;

            for (var i = 0; i < discoveryAssemblies.Length; i++)
            {
                var assembly = discoveryAssemblies[i];
                if (assembly is null)
                {
                    continue;
                }

                options.Discovery.IncludeAssembly(assembly);
            }
        });

        return hostBuilder;
    }
}
