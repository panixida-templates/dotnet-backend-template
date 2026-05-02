using Application.Abstractions.Mediator;
using Application.Users.GetDetails;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Http.Common;
using Presentation.Http.Features.Users;

namespace Presentation.Features.Users.GetDetails;

internal static class GetUserDetailsEndpoint
{
    internal const string Name = "GetUserById";
    internal const string Summary = "Get user by id";

    internal static RouteGroupBuilder MapGetByIdUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet($"/{UsersEndpoints.IdRoute}", HandleAsync)
            .WithName(Name)
            .WithSummary(Summary)
            .Produces<UserDetailsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.QueryAsync(new GetUserDetailsQuery(id), cancellationToken);

        return result.ToHttpResult(dto =>
        {
            var response = UserDetailsMapper.ToResponse(dto);
            return TypedResults.Ok(response);
        });
    }
}
