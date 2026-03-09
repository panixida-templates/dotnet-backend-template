using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;
using Application.Users.Abstractions;

using Domain.Abstractions.ResultPattern;
using Domain.Users;
using Domain.Users.Enumerations;
using Domain.Users.ValueObjects;

namespace Application.Users.Update;

public sealed class UpdateUserHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateUserCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var roleResult = UserRole.Create(command.Role);
        var nameResult = UserName.Create(command.Name);
        var emailResult = Email.Create(command.Email);
        var phoneResult = PhoneNumber.Create(command.Phone);
        var birthDateResult = BirthDate.Create(command.BirthDate);
        var avatarResult = Avatar.Create(command.Avatar);

        var validationResult = Result.Combine(
            roleResult,
            nameResult,
            emailResult,
            phoneResult,
            birthDateResult,
            avatarResult);

        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        var user = await usersRepository.GetByIdAsync(command.Id, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                Error.NotFound($"User with id '{command.Id}' was not found.")
                .WithField(nameof(User)));
        }

        user.ChangeRole(roleResult.Value);
        user.ChangeName(nameResult.Value);
        user.ChangeEmail(emailResult.Value);
        user.ChangePhone(phoneResult.Value);

        var changeBirthDateResult = user.ChangeBirthDate(birthDateResult.Value);
        if (changeBirthDateResult.IsFailure)
        {
            return changeBirthDateResult;
        }

        user.ChangeAvatar(avatarResult.Value);

        usersRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
