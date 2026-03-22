using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

using Domain.Abstractions.ResultPattern;

namespace Application.Behaviors;

public sealed class BeginTransactionBehavior<TCommand, TResult>(IUnitOfWork unitOfWork)
    : IBeforeRequestBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : Result
{
    public async Task BeforeAsync(
        TCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
    }
}
