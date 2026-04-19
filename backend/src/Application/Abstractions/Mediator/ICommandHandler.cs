namespace Application.Abstractions.Mediator;

internal interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : Result
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
