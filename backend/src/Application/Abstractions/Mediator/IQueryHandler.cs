using Domain.Abstractions.ResultPattern;

namespace Application.Abstractions.Mediator;

internal interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
    where TResult : Result
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
