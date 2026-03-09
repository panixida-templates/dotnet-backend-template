using Application.Abstractions.Mediator;

using Domain.Abstractions.ResultPattern;

namespace Application.Users.Delete;

public sealed record DeleteUserCommand(Guid Id) : ICommand<Result>;
