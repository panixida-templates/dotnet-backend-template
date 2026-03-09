using Application.Abstractions.Mediator;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Presentation.Http.Common;

namespace Presentation.Http.Features.Users.Create;

internal static class CreateUserEndpoint
{
    internal static RouteGroupBuilder MapCreateUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName("CreateUser")
            .WithSummary("Create user")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        CreateUserRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var command = CreateUserMapper.ToCommand(request, id);

        var result = await mediator.SendAsync(command, cancellationToken);

        return result.ToHttpResult(createdId =>
        {
            var response = CreateUserMapper.ToResponse(createdId);
            return TypedResults.Created($"/api/users/{createdId}", response);
        });
    }
}
