using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Organization.Product.Module.Application.Users.GetList;

using PANiXiDA.Core.Application.Messaging.Mediator;
using PANiXiDA.Core.Application.Querying.Pagination;
using PANiXiDA.Core.Application.Querying.Sorting;
using PANiXiDA.Core.Presentation.Http.Helpers;

namespace Organization.Product.Module.Presentation.Features.Users.GetList;

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
