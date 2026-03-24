using Application.Abstractions.Persistence;

using Domain.Abstractions;

namespace Infrastructure.Persistence.Ef.Core;

internal sealed class AggregateTracker : IAggregateTracker
{
    private readonly HashSet<IAggregateRoot> aggregateRoots = [];

    public void Track(IAggregateRoot aggregateRoot)
    {
        aggregateRoots.Add(aggregateRoot);
    }

    public IReadOnlyCollection<IAggregateRoot> GetAll()
    {
        return [.. aggregateRoots];
    }

    public void Clear()
    {
        aggregateRoots.Clear();
    }
}
