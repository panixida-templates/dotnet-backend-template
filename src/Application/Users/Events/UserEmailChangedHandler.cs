using Application.Abstractions.Mediator;

using Domain.Users.Events;

namespace Application.Users.Events;

public sealed class UserEmailChangedHandler : IEventHandler<UserEmailChanged>
{
    public Task HandleAsync(
        UserEmailChanged @event,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
