using Application.Abstractions.Mediator;

namespace Application.Users.Delete;

public sealed record DeleteUserCommand(Guid Id) : ICommand<Result>;
