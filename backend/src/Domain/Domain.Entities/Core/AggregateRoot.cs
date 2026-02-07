using Domain.Events.Core;

namespace Domain.Entities.Core;

public abstract class AggregateRoot<TId>(TId id) : Entity<TId>(id)
    where TId : struct
{
    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
