using Domain.Abstractions;
using Domain.Abstractions.ResultPattern;

namespace Application.Abstractions.Mediator;

public interface IMediator
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        where TResult : Result;

    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        where TResult : Result;

    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : DomainEvent;
}
