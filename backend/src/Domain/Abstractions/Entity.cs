namespace Domain.Abstractions;

public abstract class Entity<TId>(TId id) : IEntity
    where TId : struct
{
    public TId Id { get; private init; } = id;
}
