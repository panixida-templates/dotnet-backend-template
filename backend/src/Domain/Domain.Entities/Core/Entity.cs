namespace Domain.Entities.Core;

public abstract class Entity<TId>(TId id)
    where TId : struct
{
    public TId Id { get; private set; } = id;
}
