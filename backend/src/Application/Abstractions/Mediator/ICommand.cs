using Domain.Abstractions.ResultPattern;

namespace Application.Abstractions.Mediator;

public interface ICommand<out TResult>
    where TResult : Result
{
}
