using Application.Abstractions.Mediator;
using Application.Behaviors;

using Infrastructure.Mediator.Wolverine.Behaviors;
using Infrastructure.Mediator.Wolverine.Policies.Core;

using JasperFx.CodeGeneration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;

using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;

namespace Infrastructure.Mediator.Wolverine.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWolverineMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, WolverineMediator>();

        return services;
    }

    public static IHostBuilder UseWolverineMediator(
        this IHostBuilder hostBuilder,
        IConfiguration configuration,
        params Assembly[] discoveryAssemblies)
    {
        hostBuilder.UseWolverine(options =>
        {
            options.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;

            ConfigureOutbox(options, configuration);

            var registry = RequestMiddlewareRegistry.Create(builder =>
            {
                builder.AddBefore(typeof(BeginTransactionBehavior<,>));
                builder.AddAfter(typeof(SaveChangesBehavior<,>));
                builder.AddAfter(typeof(CommitTransactionBehavior<,>));
                builder.AddAfter(typeof(FlushOutgoingMessagesBehavior<,>));
                builder.AddFinally(typeof(CleanupTransactionBehavior<,>));
            });

            options.Policies.Add(new RequestMiddlewareChainPolicy(registry));

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

    private static void ConfigureOutbox(
        WolverineOptions options,
        IConfiguration configuration)
    {
        const string postgreSqlConnectionString = "PostgreSqlConnectionString";

        var connectionString = configuration.GetConnectionString(postgreSqlConnectionString);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{postgreSqlConnectionString}' was not found.");
        }

        options.PersistMessagesWithPostgresql(connectionString);
        options.UseEntityFrameworkCoreTransactions();

        options.Policies.UseDurableLocalQueues();

        options.Policies.UseDurableInboxOnAllListeners();
        options.Policies.UseDurableOutboxOnAllSendingEndpoints();
    }
}
