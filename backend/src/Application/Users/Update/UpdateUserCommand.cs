using Application.Abstractions.Mediator;

namespace Application.Users.Update;

public sealed record UpdateUserCommand(
    Guid Id,
    string Role,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar) : ICommand;
