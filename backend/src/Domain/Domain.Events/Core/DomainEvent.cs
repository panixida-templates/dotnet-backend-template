namespace Domain.Events.Core;

public abstract record DomainEvent(DateTimeOffset OccurredOn);
