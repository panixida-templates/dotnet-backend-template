using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Organization.Product.Module.Application.Users.GetList;

namespace Organization.Product.Module.Presentation.Features.Users.GetList;

internal class GetUsersListEndpoint : IEndpoint<UsersEndpoints>
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", HandleAsync)
            .WithName("GetUserList")
            .WithSummary("Get paginated user list")
            .Produces<UserListItemResponse>(StatusCodes.Status200OK);
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

        return result.ToHttpResult(item 
            => TypedResults.Ok(GetUsersListMapper.ToResponse(item)));
    }
}
