using Application.Abstractions.Mediator;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Presentation.Http.Features.Users.Update;

internal static class UpdateUserEndpoint
{
    internal static RouteGroupBuilder MapUpdateUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", HandleAsync)
            .WithName("UpdateUser")
            .WithSummary("Update user")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateUserRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = UpdateUserMapper.ToCommand(request, id);
        await mediator.SendAsync(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
