using Application.Abstractions.Mediator;

namespace Application.Users.Create;

public sealed record CreateUserCommand(
    Guid Id,
    string Role,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar) : ICommand<Guid>;
