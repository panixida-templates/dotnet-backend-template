namespace Domain.Abstractions;

public interface IAggregateRoot : IEntity
{
    IReadOnlyCollection<DomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}
