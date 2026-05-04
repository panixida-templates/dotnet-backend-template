namespace Infrastructure.Ef.Core.Read;

internal abstract class AuditableReadDbModel<TId> : ReadDbModel<TId>
    where TId : struct
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
