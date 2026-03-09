namespace Domain.Abstractions;

public abstract class AggregateRoot<TId>(TId id) : Entity<TId>(id), IAggregateRoot
    where TId : struct
{
    private readonly List<DomainEvent> _domainEvents = [];

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IReadOnlyCollection<DomainEvent> GetDomainEvents()
    {
        return _domainEvents.AsReadOnly();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
