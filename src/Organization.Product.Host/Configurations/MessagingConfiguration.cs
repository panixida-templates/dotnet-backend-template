using Organization.Product.Module.Application;
using Organization.Product.Module.Infrastructure.Persistence.Core;

using PANiXiDA.Core.Infrastructure.Messaging.Wolverine.DependencyInjection;
using PANiXiDA.Core.Infrastructure.Persistence.Ef.Constants;

namespace Organization.Product.Host.Configurations;

internal static class MessagingConfiguration
{
    internal static WebApplicationBuilder AddMessaging(this WebApplicationBuilder builder)
    {
        var connectionString =
            builder.Configuration.GetConnectionString(EfConstants.PostgreSqlConnectionStringName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{EfConstants.PostgreSqlConnectionStringName}' was not found.");

        builder.Host.UseWolverineMediator(
            messageStoreConnectionString: connectionString,
            configureModules: modules => modules.AddModule<ModuleWriteDbContext>(
                ApplicationAssembly.Instance,
                typeof(ModuleWriteDbContext).Assembly));

        return builder;
    }
}
