using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Organization.Product.Module.Presentation.Features.Users.GetDetails;

using PANiXiDA.Core.Application.Messaging.Mediator;
using PANiXiDA.Core.Presentation.Http.Endpoints;
using PANiXiDA.Core.Presentation.Http.Helpers;

namespace Organization.Product.Module.Presentation.Features.Users.Create;

internal class CreateUserEndpoint : IEndpoint<UsersEndpoints>
{
    internal const string Name = "CreateUser";
    internal const string Summary = "Create user";

    public void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName(Name)
            .WithSummary(Summary)
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        CreateUserRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = CreateUserMapper.ToCommand(request);
        var result = await mediator.SendAsync(command, cancellationToken);

        return result.ToHttpResult(createdId =>
        {
            var response = CreateUserMapper.ToResponse(createdId);
            return TypedResults.CreatedAtRoute(
                response,
                GetUserDetailsEndpoint.Name,
                new { id = createdId });
        });
    }
}
