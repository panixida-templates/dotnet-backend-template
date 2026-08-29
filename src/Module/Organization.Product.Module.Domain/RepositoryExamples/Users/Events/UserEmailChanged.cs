namespace Organization.Product.Module.Domain.RepositoryExamples.Users.Events;

public sealed record UserEmailChanged(
    Guid UserId,
    string OldEmail,
    string NewEmail) : DomainEvent;
