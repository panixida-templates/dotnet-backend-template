using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;
using Application.Users.Abstractions;

using Domain.Users;
using Domain.Users.Enumerations;
using Domain.Users.ValueObjects;

namespace Application.Users.Create;

public sealed class CreateUserHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = User.Create(
            id: command.Id,
            role: UserRole.FromName(command.Role),
            name: UserName.Create(command.Name),
            email: Email.Create(command.Email),
            phone: PhoneNumber.Create(command.Phone),
            birthDate: BirthDate.Create(command.Birthday),
            avatar: Avatar.Create(command.Avatar));

        usersRepository.Add(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
