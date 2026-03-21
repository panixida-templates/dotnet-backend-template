using Domain.Abstractions.ResultPattern;

namespace Application.Abstractions.Mediator;

public interface IBeforeRequestBehavior<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : Result
{
    Task BeforeAsync(
        TRequest request,
        CancellationToken cancellationToken);
}
