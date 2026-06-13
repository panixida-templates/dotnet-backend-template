using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Organization.Product.Module.Presentation.Features.Users.GetDetails;

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
            TypedResults.CreatedAtRoute(
                CreateUserMapper.ToResponse(createdId),
                GetUserDetailsEndpoint.Name,
                new { id = createdId }));
    }
}
