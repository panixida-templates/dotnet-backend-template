using Organization.Product.Module.Infrastructure.DependencyInjection;

namespace Organization.Product.Host.Configurations.Modules;

internal static class ModuleModuleConfiguration
{
    internal static WebApplicationBuilder AddModuleModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration);

        return builder;
    }
}
