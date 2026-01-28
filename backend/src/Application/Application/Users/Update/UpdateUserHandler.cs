using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

namespace Application.Users.Update;

internal sealed class UpdateUserHandler(IUsersRepository usersRepository) : ICommandHandler<UpdateUserCommand>
{
    public Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        return usersRepository.AddOrUpdateAsync(command.Data);
    }
}
