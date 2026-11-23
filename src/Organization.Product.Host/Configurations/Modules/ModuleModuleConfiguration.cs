using Organization.Product.Module.Infrastructure.DependencyInjection;
using Organization.Product.Module.Presentation.DependencyInjection;

namespace Organization.Product.Host.Configurations.Modules;

internal static class ModuleModuleConfiguration
{
    internal static WebApplicationBuilder AddModuleModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddPresentation(builder.Configuration);
        builder.Host.UseInfrastructure(builder.Configuration);

        return builder;
    }
}
