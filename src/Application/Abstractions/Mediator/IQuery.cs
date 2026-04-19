namespace Application.Abstractions.Mediator;

public interface IQuery<out TResult> : IRequest<TResult>
    where TResult : Result
{
}
