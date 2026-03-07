namespace Infrastructure.Persistence.Ef.Core;

internal abstract class AuditableDbModel<TId> : DbModel<TId>
    where TId : struct
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
