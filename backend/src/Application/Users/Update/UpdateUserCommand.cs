using Application.Abstractions.Mediator;

using Domain.Abstractions.ResultPattern;

namespace Application.Users.Update;

public sealed record UpdateUserCommand(
    Guid Id,
    string Role,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar) : ICommand<Result>;
