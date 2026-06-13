using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Organization.Product.Module.Presentation.Features.Users.Update;

internal class UpdateUserEndpoint : IEndpoint<UsersEndpoints>
{
    internal const string Name = "UpdateUser";
    internal const string Summary = "Update user";

    public void Map(RouteGroupBuilder group)
    {
        group.MapPut($"/{UsersEndpoints.IdRoute}", HandleAsync)
            .WithName(Name)
            .WithSummary(Summary)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
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
