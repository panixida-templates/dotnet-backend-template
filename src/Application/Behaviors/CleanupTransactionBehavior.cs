using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

namespace Application.Behaviors;

public sealed class CleanupTransactionBehavior<TCommand, TResult>(IUnitOfWork unitOfWork)
    : IFinallyRequestBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : Result
{
    public async Task FinallyAsync(
        TCommand request,
        TResult? result,
        Exception? exception,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!unitOfWork.HasActiveTransaction)
            {
                return;
            }

            var shouldRollback =
                exception is not null ||
                result is null ||
                !result.IsSuccess;

            if (shouldRollback)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
            }
        }
        finally
        {
            await unitOfWork.DisposeTransactionAsync();
        }
    }
}
