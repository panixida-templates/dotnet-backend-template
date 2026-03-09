using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;
using Application.Users.Abstractions;

using Domain.Abstractions.ResultPattern;
using Domain.Users;
using Domain.Users.Enumerations;
using Domain.Users.ValueObjects;

namespace Application.Users.Create;

public sealed class CreateUserHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var idResult = UserId.Create(command.Id);
        var roleResult = UserRole.Create(command.Role);
        var nameResult = UserName.Create(command.Name);
        var emailResult = Email.Create(command.Email);
        var phoneResult = PhoneNumber.Create(command.Phone);
        var birthDateResult = BirthDate.Create(command.BirthDate);
        var avatarResult = Avatar.Create(command.Avatar);

        return await Result.Combine(
                idResult,
                roleResult,
                nameResult,
                emailResult,
                phoneResult,
                birthDateResult,
                avatarResult)
            .Bind(() =>
            {
                return User.Create(
                    idResult.Value,
                    roleResult.Value,
                    nameResult.Value,
                    emailResult.Value,
                    phoneResult.Value,
                    birthDateResult.Value,
                    avatarResult.Value);
            })
            .Tap(usersRepository.Add)
            .BindAsync(async user =>
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success(user.Id.Value);
            });
    }
}
