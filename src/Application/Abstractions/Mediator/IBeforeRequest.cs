namespace Application.Abstractions.Mediator;

public interface IBeforeRequest<in TRequest>
{
    Task BeforeAsync(TRequest request, CancellationToken cancellationToken);
}
