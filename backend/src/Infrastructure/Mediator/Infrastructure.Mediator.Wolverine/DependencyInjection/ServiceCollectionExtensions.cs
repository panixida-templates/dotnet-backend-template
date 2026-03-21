using Application.Abstractions.Mediator;
using Application.Behaviors;

using Infrastructure.Mediator.Wolverine.Policies;

using JasperFx.CodeGeneration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;

using Wolverine;

namespace Infrastructure.Mediator.Wolverine.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWolverineMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, WolverineMediator>();

        return services;
    }

    public static IHostBuilder UseWolverineMediator(this IHostBuilder hostBuilder, params Assembly[] discoveryAssemblies)
    {
        hostBuilder.UseWolverine(options =>
        {
            options.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;

            options.Durability.Mode = DurabilityMode.MediatorOnly;

            var registry = new AfterCommandMiddlewareRegistry(
                typeof(CommitUnitOfWorkBehavior<,>)
            );

            options.Policies.Add(new AfterCommandChainPolicy(registry));

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

public sealed class AfterCommandMiddlewareRegistry(params Type[] middlewareTypes)
{
    public IReadOnlyList<Type> MiddlewareTypes { get; } = middlewareTypes;
}
