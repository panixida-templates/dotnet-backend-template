using Application.Abstractions.Mediator;
using Application.Users.Abstractions;

using Domain.Users;

namespace Application.Users.Create;

public sealed class CreateUserHandler(IUsersRepository usersRepository)
    : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    public Task<Result<Guid>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userResult = User.Create(
            command.Role,
            command.Name,
            command.Email,
            command.Phone,
            command.BirthDate,
            command.Avatar);

        if (userResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Guid>(userResult.Errors));
        }

        usersRepository.Add(userResult.Value);

        return Task.FromResult(Result.Success(userResult.Value.Id.Value));
    }
}
