using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

namespace Application.Behaviors;

public sealed class CommitTransactionBehavior<TCommand, TResult>(IUnitOfWork unitOfWork)
    : IAfterRequestBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : Result
{
    public async Task AfterAsync(
        TCommand request,
        TResult result,
        CancellationToken cancellationToken)
    {
        if (!unitOfWork.HasActiveTransaction)
        {
            return;
        }

        if (!result.IsSuccess)
        {
            return;
        }

        await unitOfWork.CommitTransactionAsync(cancellationToken);
    }
}
