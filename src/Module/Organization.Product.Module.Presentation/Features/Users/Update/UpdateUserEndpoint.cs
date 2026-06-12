using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using PANiXiDA.Core.Application.Messaging.Mediator;
using PANiXiDA.Core.Presentation.Http.Helpers;

namespace Organization.Product.Module.Presentation.Features.Users.Update;

internal static class UpdateUserEndpoint
{
    internal const string Name = "UpdateUser";
    internal const string Summary = "Update user";

    internal static RouteGroupBuilder MapUpdateUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut($"/{UsersEndpoints.IdRoute}", HandleAsync)
            .WithName(Name)
            .WithSummary(Summary)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateUserRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = UpdateUserMapper.ToCommand(request, id);
        var result = await mediator.SendAsync(command, cancellationToken);

        return result.ToHttpResult(TypedResults.NoContent);
    }
}
