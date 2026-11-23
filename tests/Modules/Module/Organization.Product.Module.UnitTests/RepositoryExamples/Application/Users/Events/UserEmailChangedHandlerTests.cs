using Organization.Product.Module.Application.RepositoryExamples.Users.Events;
using Organization.Product.Module.Domain.RepositoryExamples.Users.Events;

namespace Organization.Product.Module.UnitTests.RepositoryExamples.Application.Users.Events;

public sealed class UserEmailChangedHandlerTests
{
    [Fact(DisplayName = "HandleAsync should complete when domain event is provided")]
    public async Task HandleAsync_Should_Complete_When_Domain_Event_Is_Provided()
    {
        var handler = new UserEmailChangedHandler();
        var domainEvent = new UserEmailChanged(
            Guid.NewGuid(),
            "old@example.com",
            "new@example.com");

        var task = handler.HandleAsync(domainEvent, CancellationToken.None);

        task.IsCompletedSuccessfully.ShouldBeTrue();
        await task;
    }
}
