using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;
using Application.Users.Abstractions;

using Domain.Abstractions.ResultPattern;
using Domain.Users;

namespace Application.Users.Delete;

public sealed class DeleteUserHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteUserCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(command.Id, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                Error.NotFound($"User with id '{command.Id}' was not found.")
                .WithField(nameof(User)));
        }

        usersRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
