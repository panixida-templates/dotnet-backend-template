using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PANiXiDA.Core.Presentation.SignalR.DependencyInjection;

namespace Organization.Product.Module.Presentation.DependencyInjection;

public static class ServiceCollectionExtensions
{
    internal const string SignalREnabledConfigurationKey = "SignalR:Enabled";

    public static IServiceCollection AddPresentation(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddHttp(configuration, typeof(ServiceCollectionExtensions).Assembly);

        if (configuration.GetValue<bool>(SignalREnabledConfigurationKey))
        {
            serviceCollection.AddSignalR(configuration);
        }

        return serviceCollection;
    }
}
