using Application.Abstractions.Mediator;

using Infrastructure.Ef.EfCore;

using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace Infrastructure.Mediator.Wolverine;

public sealed class WolverineMediator(
    IMessageBus messageBus,
    IDbContextOutbox<TemplateWriteDbContext> outbox) : IMediator
{
    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        where TResult : Result
    {
        return messageBus.InvokeAsync<TResult>(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        where TResult : Result
    {
        return messageBus.InvokeAsync<TResult>(query, cancellationToken);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : DomainEvent
    {
        await outbox.PublishAsync(@event);
    }
}
