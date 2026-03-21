using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

using Presentation.Http.Features.Users;
using Presentation.Http.Middlewares;

namespace Presentation.Http.Bootstrapper;

public static class EndpointRouteBuilderExtensions
{
    public static WebApplication MapHttp(this WebApplication app)
    {
        app.UseForwardedHeaders();
        app.UseExceptionHandler();
        app.UseMiddleware<LoggingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
                options.RoutePrefix = "swagger";
            });
        }

        MapFeatureEndpoints(app);

        return app;
    }

    private static void MapFeatureEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapUsersEndpoints();
    }
}
