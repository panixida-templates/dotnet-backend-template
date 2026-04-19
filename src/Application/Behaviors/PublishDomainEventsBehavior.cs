using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

namespace Application.Behaviors;

public sealed class PublishDomainEventsBehavior<TRequest, TResult>(
    IMediator mediator,
    IAggregateTracker aggregateTracker) : IAfterRequestBehavior<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : Result
{
    public async Task AfterAsync(
        TRequest request,
        TResult result,
        CancellationToken cancellationToken)
    {
        var aggregateRoots = aggregateTracker.GetAll();

        if (result.IsFailure)
        {
            ClearDomainEvents(aggregateRoots);
            aggregateTracker.Clear();
            return;
        }

        foreach (var aggregateRoot in aggregateRoots)
        {
            var domainEvents = aggregateRoot.GetDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                await mediator.PublishAsync(domainEvent, cancellationToken);
            }
        }

        ClearDomainEvents(aggregateRoots);
        aggregateTracker.Clear();
    }

    private static void ClearDomainEvents(IReadOnlyCollection<IAggregateRoot> aggregateRoots)
    {
        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearDomainEvents();
        }
    }
}
