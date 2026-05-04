using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence.Read.Paged;
using Application.Abstractions.Persistence.Read.Sorting;
using Application.Users.GetList;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Presentation.Http.Common;

namespace Presentation.Features.Users.GetList;

internal static class GetUsersListEndpoint
{
    internal static RouteGroupBuilder MapGetUsersListEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", HandleAsync)
            .WithName("GetUserList")
            .WithSummary("Get paginated user list")
            .Produces<UserListItemResponse>(StatusCodes.Status200OK);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetUsersListRequest request,
        [AsParameters] PaginationParameters paginationParameters,
        [AsParameters] SortParameters sortParameters,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var filterParameters = GetUsersListMapper.ToFilterParameters(request);

        var result = await mediator.QueryAsync(
            new GetUsersListQuery(filterParameters, paginationParameters, sortParameters),
            cancellationToken);

        return result.ToHttpResult(item => TypedResults.Ok(GetUsersListMapper.ToResponse(item)));
    }
}
