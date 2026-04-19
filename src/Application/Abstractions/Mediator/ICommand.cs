namespace Application.Abstractions.Mediator;

public interface ICommand<out TResult> : IRequest<TResult>
    where TResult : Result
{
}
