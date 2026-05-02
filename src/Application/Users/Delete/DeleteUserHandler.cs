using Application.Abstractions.Mediator;
using Application.Users.Abstractions;

using Domain.Users;

namespace Application.Users.Delete;

public sealed class DeleteUserHandler(IUsersRepository usersRepository)
    : ICommandHandler<DeleteUserCommand, Result>
{
    public async Task<Result> HandleAsync(
        DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = (await UserId.Create(command.Id)
            .BindAsync(async userId =>
            {
                var user = await usersRepository.GetByIdAsync(
                    userId,
                    cancellationToken);

                return user is null
                    ? Result.Failure<User>(Error.NotFound($"User with id '{command.Id}' was not found."))
                    : Result.Success(user);
            }))
            .Tap(usersRepository.Delete);

        return result;
    }
}
