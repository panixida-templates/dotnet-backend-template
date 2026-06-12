using Organization.Product.Module.Application.Users.Abstractions;
using Organization.Product.Module.Domain.Users;

using PANiXiDA.Core.Application.Messaging.Mediator.Handlers;

namespace Organization.Product.Module.Application.Users.Delete;

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
