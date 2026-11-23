namespace Organization.Product.Module.Application.RepositoryExamples.Users.Write.Update;

public sealed record UpdateUserCommand(
    Guid Id,
    string Role,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar) : ICommand<Result>;
