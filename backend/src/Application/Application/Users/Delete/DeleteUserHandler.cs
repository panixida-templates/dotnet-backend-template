using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

namespace Application.Users.Delete;

public sealed class DeleteUserHandler(IUsersRepository usersRepository) : ICommandHandler<DeleteUserCommand>
{
    public Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        return usersRepository.DeleteAsync(command.Id);
    }
}
