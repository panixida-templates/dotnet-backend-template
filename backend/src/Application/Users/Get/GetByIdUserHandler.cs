using Application.Abstractions.Mediator;
using Application.Users.Abstractions;

namespace Application.Users.Get;

public sealed class GetByIdUserHandler(IUsersQueryService usersQueryService) : IQueryHandler<GetByIdUserQuery, GetByIdUserDto>
{
    public Task<GetByIdUserDto> HandleAsync(GetByIdUserQuery query, CancellationToken cancellationToken)
    {
        return usersQueryService.GetByIdAsync(query.Id, cancellationToken);
    }
}
