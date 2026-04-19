//using Application.Abstractions.Mediator;
//using Application.Abstractions.Queries;
//using Application.Users.Search;

//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Routing;

//namespace Presentation.Http.Features.Users.Search;

//internal static class SearchUsersEndpoint
//{
//    internal static RouteGroupBuilder MapSearchUsersEndpoint(this RouteGroupBuilder group)
//    {
//        group.MapGet("/", HandleAsync)
//            .WithName("SearchUsers")
//            .WithSummary("Search users")
//            .Produces<SearchUsersResponse>(StatusCodes.Status200OK);

//        return group;
//    }

//    private static async Task<IResult> HandleAsync(
//        [AsParameters] UsersSearchParams searchParams,
//        IMediator mediator,
//        CancellationToken cancellationToken)
//    {
//        var result = await mediator.QueryAsync(
//            new SearchUsersQuery(searchParams),
//            cancellationToken);

//        var response = SearchUsersMapper.ToResponse(result);

//        return TypedResults.Ok(response);
//    }
//}
