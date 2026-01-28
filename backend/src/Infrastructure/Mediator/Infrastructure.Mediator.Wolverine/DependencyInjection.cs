using System.Reflection;

using Application.Abstractions.Mediator;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wolverine;

namespace Infrastructure.Mediator.Wolverine;

public static class DependencyInjection
{
    public static IServiceCollection AddWolverineMediator(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IMediator, WolverineMediator>();

        return services;
    }

    public static IHostBuilder UseWolverineMediator(
        this IHostBuilder hostBuilder,
        bool mediatorOnly,
        params Assembly[] discoveryAssemblies)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        var settings = new WolverineMediatorSettings(mediatorOnly, discoveryAssemblies);
        var configurator = new WolverineMediatorConfigurator(settings);

        hostBuilder.UseWolverine(configurator.Configure);

        return hostBuilder;
    }

    public static IHostBuilder UseWolverineMediator(
        this IHostBuilder hostBuilder,
        params Assembly[] discoveryAssemblies)
    {
        return hostBuilder.UseWolverineMediator(mediatorOnly: true, discoveryAssemblies: discoveryAssemblies);
    }

    private sealed class WolverineMediatorSettings
    {
        public WolverineMediatorSettings(bool mediatorOnly, Assembly[] discoveryAssemblies)
        {
            MediatorOnly = mediatorOnly;
            DiscoveryAssemblies = discoveryAssemblies ?? Array.Empty<Assembly>();
        }

        public bool MediatorOnly { get; }

        public Assembly[] DiscoveryAssemblies { get; }
    }

    private sealed class WolverineMediatorConfigurator
    {
        private readonly WolverineMediatorSettings _settings;

        public WolverineMediatorConfigurator(WolverineMediatorSettings settings)
        {
            _settings = settings;
        }

        public void Configure(WolverineOptions opts)
        {
            if (_settings.MediatorOnly)
            {
                opts.Durability.Mode = DurabilityMode.MediatorOnly;
            }

            for (var i = 0; i < _settings.DiscoveryAssemblies.Length; i++)
            {
                var assembly = _settings.DiscoveryAssemblies[i];
                if (assembly is null)
                {
                    continue;
                }

                opts.Discovery.IncludeAssembly(assembly);
            }
        }
    }
}
