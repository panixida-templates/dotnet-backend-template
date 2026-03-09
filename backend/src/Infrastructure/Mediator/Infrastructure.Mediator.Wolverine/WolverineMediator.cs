using Application.Abstractions.Mediator;

using Domain.Abstractions;
using Domain.Abstractions.ResultPattern;

using Wolverine;

namespace Infrastructure.Mediator.Wolverine;

public sealed class WolverineMediator(IMessageBus messageBus) : IMediator
{
    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        where TResult : Result
    {
        return messageBus.InvokeAsync<TResult>(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return messageBus.InvokeAsync<TResult>(query, cancellationToken);
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : DomainEvent
    {
        return messageBus.PublishAsync(@event).AsTask();
    }
}
