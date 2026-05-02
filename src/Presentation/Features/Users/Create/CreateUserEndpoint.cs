using Application.Abstractions.Mediator;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Features.Users.GetDetails;
using Presentation.Http.Common;

namespace Presentation.Http.Features.Users.Create;

internal static class CreateUserEndpoint
{
    internal const string Name = "CreateUser";
    internal const string Summary = "Create user";

    internal static RouteGroupBuilder MapCreateUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName(Name)
            .WithSummary(Summary)
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        CreateUserRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = CreateUserMapper.ToCommand(request);
        var result = await mediator.SendAsync(command, cancellationToken);

        return result.ToHttpResult(createdId =>
        {
            var response = CreateUserMapper.ToResponse(createdId);
            return TypedResults.CreatedAtRoute(
                response,
                GetUserDetailsEndpoint.Name,
                new { id = createdId });
        });
    }
}
