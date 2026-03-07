namespace Domain.Abstractions;

public abstract record DomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.CreateVersion7();
        OccurredOnUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private init; }
    public DateTimeOffset OccurredOnUtc { get; private init; }
}
