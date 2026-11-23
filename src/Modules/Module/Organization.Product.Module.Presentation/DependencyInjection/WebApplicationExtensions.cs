using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

using PANiXiDA.Core.Presentation.SignalR.DependencyInjection;

namespace Organization.Product.Module.Presentation.DependencyInjection;

public static class WebApplicationExtensions
{
    public static WebApplication UsePresentation(
        this WebApplication app)
    {
        if (app.Configuration.GetValue<bool>(ServiceCollectionExtensions.SignalREnabledConfigurationKey))
        {
            app.UseSignalR(typeof(WebApplicationExtensions).Assembly);
        }

        app.UseHttp();

        return app;
    }
}
