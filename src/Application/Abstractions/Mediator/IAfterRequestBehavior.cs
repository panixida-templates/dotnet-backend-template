namespace Application.Abstractions.Mediator;

public interface IAfterRequestBehavior<TRequest, in TResult>
    where TRequest : IRequest<TResult>
    where TResult : Result
{
    Task AfterAsync(
        TRequest request,
        TResult result,
        CancellationToken cancellationToken);
}
