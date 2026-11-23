using Microsoft.AspNetCore.Builder;

namespace Organization.Product.Module.Presentation.DependencyInjection;

public static class WebApplicationExtensions
{
    public static WebApplication UsePresentation(
        this WebApplication app)
    {
        app.UseHttp();

        return app;
    }
}
