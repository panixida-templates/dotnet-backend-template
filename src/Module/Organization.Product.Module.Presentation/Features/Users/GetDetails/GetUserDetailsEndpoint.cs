using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Organization.Product.Module.Application.Users.GetDetails;

namespace Organization.Product.Module.Presentation.Features.Users.GetDetails;

internal class GetUserDetailsEndpoint : IEndpoint<UsersEndpoints>
{
    internal const string Name = "GetUserById";
    internal const string Summary = "Get user by id";

    public void Map(RouteGroupBuilder group)
    {
        group.MapGet($"/{UsersEndpoints.IdRoute}", HandleAsync)
            .WithName(Name)
            .WithSummary(Summary)
            .Produces<UserDetailsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.QueryAsync(
            new GetUserDetailsQuery(id), cancellationToken);

        return result.ToHttpResult(dto =>
            TypedResults.Ok(UserDetailsMapper.ToResponse(dto)));
    }
}
