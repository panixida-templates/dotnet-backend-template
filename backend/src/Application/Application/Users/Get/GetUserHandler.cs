using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

using Domain.Entities;

namespace Application.Users.Get;

public sealed class GetUserHandler(IUsersRepository usersRepository) : IQueryHandler<GetUserQuery, User>
{
    public Task<User> HandleAsync(GetUserQuery query, CancellationToken cancellationToken)
    {
        return usersRepository.GetAsync(query.Id, query.ConvertParams);
    }
}
