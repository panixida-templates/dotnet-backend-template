using Organization.Product.Module.Domain.RepositoryExamples.Users.Events;

namespace Organization.Product.Module.Application.RepositoryExamples.Users.Events;

public sealed class UserEmailChangedHandler : IEventHandler<UserEmailChanged>
{
    public Task HandleAsync(
        UserEmailChanged @event,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
