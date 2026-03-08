using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;
using Application.Users.Abstractions;

using Domain.Users.Enumerations;
using Domain.Users.ValueObjects;

namespace Application.Users.Update;

public sealed class UpdateUserHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateUserCommand>
{
    public async Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(command.Id, cancellationToken);

        user.ChangeRole(UserRole.FromName(command.Role));
        user.ChangeName(UserName.Create(command.Name));
        user.ChangeEmail(Email.Create(command.Email));
        user.ChangePhone(PhoneNumber.Create(command.Phone));
        user.ChangeBirthDate(BirthDate.Create(command.BirthDate));
        user.ChangeAvatar(Avatar.Create(command.Avatar));

        usersRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
