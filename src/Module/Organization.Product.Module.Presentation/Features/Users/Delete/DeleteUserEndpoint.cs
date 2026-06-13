using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Organization.Product.Module.Application.Users.Delete;

namespace Organization.Product.Module.Presentation.Features.Users.Delete;

internal static class DeleteUserEndpoint
{
    internal const string Name = "DeleteUser";
    internal const string Summary = "Delete user";

    internal static RouteGroupBuilder MapDeleteUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete($"/{UsersEndpoints.IdRoute}", HandleAsync)
            .WithName(Name)
            .WithSummary(Summary)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(new DeleteUserCommand(id), cancellationToken);

        return result.ToHttpResult(TypedResults.NoContent);
    }
}
