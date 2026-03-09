using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddValidation();
        services.AddOpenApi();

        return services;
    }
}