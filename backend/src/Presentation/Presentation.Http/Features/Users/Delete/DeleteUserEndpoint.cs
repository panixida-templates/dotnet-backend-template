using Application.Abstractions.Mediator;
using Application.Users.Delete;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Presentation.Http.Features.Users.Delete;

internal static class DeleteUserEndpoint
{
    internal static RouteGroupBuilder MapDeleteUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", HandleAsync)
            .WithName("DeleteUser")
            .WithSummary("Delete user")
            .Produces(StatusCodes.Status204NoContent);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.SendAsync(new DeleteUserCommand(id), cancellationToken);
        return TypedResults.NoContent();
    }
}
