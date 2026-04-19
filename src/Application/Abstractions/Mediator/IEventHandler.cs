namespace Application.Abstractions.Mediator;

public interface IEventHandler<in TEvent>
    where TEvent : DomainEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
