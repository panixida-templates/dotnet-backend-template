using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

using Domain.Abstractions.ResultPattern;

namespace Application.Behaviors;

public sealed class CommitUnitOfWorkBehavior<TCommand, TResult>(IUnitOfWork unitOfWork)
    : IAfterRequestBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : Result
{
    public async Task AfterAsync(
        TCommand request,
        TResult result,
        CancellationToken cancellationToken)
    {
        if (result.IsSuccess)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
