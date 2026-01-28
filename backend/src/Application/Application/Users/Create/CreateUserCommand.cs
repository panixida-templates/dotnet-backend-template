using Application.Abstractions.Mediator;

using Domain.Entities;

namespace Application.Users.Create;

public sealed record CreateUserCommand(User Data) : ICommand<Guid>;
