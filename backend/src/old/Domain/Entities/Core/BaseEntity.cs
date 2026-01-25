namespace Entities.Core;

public abstract class BaseEntity<TId>(TId id)
    where TId : struct
{
    public TId Id { get; set; } = id;
}
