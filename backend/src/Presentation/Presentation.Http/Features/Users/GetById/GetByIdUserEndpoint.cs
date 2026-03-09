using Application.Abstractions.Mediator;
using Application.Users.Get;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Presentation.Http.Common;

namespace Presentation.Http.Features.Users.GetById;

internal static class GetByIdUserEndpoint
{
    internal static RouteGroupBuilder MapGetByIdUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleAsync)
            .WithName("GetUserById")
            .WithSummary("Get user by id")
            .Produces<GetByIdUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.QueryAsync(new GetByIdUserQuery(id), cancellationToken);

        return result.ToHttpResult(dto =>
        {
            var response = GetByIdUserMapper.ToResponse(dto);
            return TypedResults.Ok(response);
        });
    }
}