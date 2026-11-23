using Organization.Product.Host.Common;
using Organization.Product.Module.Presentation;

using PANiXiDA.Core.Presentation.Http.DependencyInjection;

using HttpServiceCollectionExtensions = PANiXiDA.Core.Presentation.Http.DependencyInjection.ServiceCollectionExtensions;

namespace Organization.Product.Host.Configurations;

internal static class HttpConfiguration
{
    internal static WebApplicationBuilder AddHttp(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(
            options =>
            {
                options.Limits.MaxRequestBodySize = FilesConstants.FileRequestSizeLimit;
            });

        builder.Services.AddHttp(builder.Configuration, PresentationAssembly.Instance);

        return builder;
    }

    internal static WebApplication UseHttp(this WebApplication app)
    {
        HttpServiceCollectionExtensions.UseHttp(app);

        return app;
    }
}
