namespace Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read;

public sealed class UserReadDbModel : AuditableReadDbModel<Guid>
{
    public required string Role { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? Avatar { get; set; }
}
