using Application.Abstractions.Mediator;
using Application.Users.Abstractions;

using Domain.Abstractions.ResultPattern;

namespace Application.Users.Get;

public sealed class GetByIdUserHandler(IUsersQueryService usersQueryService)
    : IQueryHandler<GetByIdUserQuery, Result<GetByIdUserDto>>
{
    public async Task<Result<GetByIdUserDto>> HandleAsync(GetByIdUserQuery query, CancellationToken cancellationToken)
    {
        var user = await usersQueryService.GetByIdAsync(query.Id, cancellationToken);
        if (user is null)
        {
            return Result.Failure<GetByIdUserDto>(Error.NotFound($"User with id '{query.Id}' was not found."));
        }

        return Result.Success(user);
    }
}
