using Asp.Versioning;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Presentation.Http.Middlewares;

using System.Text.Json;

namespace Presentation.Http.Bootstrapper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttp(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                if (context.ProblemDetails is not HttpValidationProblemDetails validationProblem)
                {
                    return;
                }

                var normalizedErrors = validationProblem.Errors.ToDictionary(
                    item => JsonNamingPolicy.CamelCase.ConvertName(item.Key),
                    item => item.Value);

                var normalizedProblem = new HttpValidationProblemDetails(normalizedErrors)
                {
                    Title = validationProblem.Title,
                    Type = validationProblem.Type,
                    Status = validationProblem.Status,
                    Detail = validationProblem.Detail,
                    Instance = validationProblem.Instance
                };

                foreach (var extension in validationProblem.Extensions)
                {
                    normalizedProblem.Extensions[extension.Key] = extension.Value;
                }

                context.ProblemDetails = normalizedProblem;
            };
        });
        services.AddExceptionHandler<ExceptionHandler>();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = false;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddValidation();
        services.AddOpenApi();

        return services;
    }
}