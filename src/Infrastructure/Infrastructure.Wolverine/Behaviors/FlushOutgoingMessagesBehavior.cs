using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

using Infrastructure.Persistence.Ef.EfCore;

using Wolverine.EntityFrameworkCore;

namespace Infrastructure.Mediator.Wolverine.Behaviors;

public sealed class FlushOutgoingMessagesBehavior<TCommand, TResult>(
    IUnitOfWork unitOfWork,
    IDbContextOutbox<DefaultDbContext> outbox)
    : IAfterRequestBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : Result
{
    public async Task AfterAsync(
        TCommand request,
        TResult result,
        CancellationToken cancellationToken)
    {
        if (!result.IsSuccess)
        {
            return;
        }

        if (unitOfWork.HasActiveTransaction)
        {
            return;
        }

        await outbox.FlushOutgoingMessagesAsync();
    }
}
