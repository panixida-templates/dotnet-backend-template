using Organization.Product.Module.Domain.Users.Events;

using PANiXiDA.Core.Application.Messaging.EventBus.Handlers;

namespace Organization.Product.Module.Application.Users.Events;

public sealed class UserEmailChangedHandler : IEventHandler<UserEmailChanged>
{
    public Task HandleAsync(
        UserEmailChanged @event,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
