using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Http.Bootstrapper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttp(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddValidation();
        services.AddOpenApi();

        return services;
    }
}
