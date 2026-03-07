using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;
using Application.Users.Abstractions;

namespace Application.Users.Delete;

public sealed class DeleteUserHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteUserCommand>
{
    public async Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(command.Id, cancellationToken);
        usersRepository.Delete(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

