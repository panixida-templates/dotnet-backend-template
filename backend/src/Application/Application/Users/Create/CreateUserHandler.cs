using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

namespace Application.Users.Create;

public sealed class CreateUserHandler(IUsersRepository usersRepository) : ICommandHandler<CreateUserCommand, Guid>
{
    public Task<Guid> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        return usersRepository.AddOrUpdateAsync(command.Data);
    }
}
