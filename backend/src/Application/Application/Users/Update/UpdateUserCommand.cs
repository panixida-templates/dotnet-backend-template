using Application.Abstractions.Mediator;

using Domain.Entities;

namespace Application.Users.Update;

public sealed record UpdateUserCommand(Guid Id, User Data) : ICommand;
