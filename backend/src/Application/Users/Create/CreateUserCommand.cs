using Application.Abstractions.Mediator;

using Domain.Abstractions.ResultPattern;

namespace Application.Users.Create;

public sealed record CreateUserCommand(
    string Role,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar) : ICommand<Result<Guid>>;
