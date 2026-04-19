namespace Infrastructure.Mediator.Wolverine.Policies.Core;

public sealed class RequestMiddlewareRegistry(
    IReadOnlyList<Type> beforeMiddlewareTypes,
    IReadOnlyList<Type> afterMiddlewareTypes,
    IReadOnlyList<Type> finallyMiddlewareTypes)
{
    public IReadOnlyList<Type> BeforeMiddlewareTypes { get; } = beforeMiddlewareTypes;
    public IReadOnlyList<Type> AfterMiddlewareTypes { get; } = afterMiddlewareTypes;
    public IReadOnlyList<Type> FinallyMiddlewareTypes { get; } = finallyMiddlewareTypes;

    public static RequestMiddlewareRegistry Empty { get; } = new(
        [],
        [],
        []);

    public static RequestMiddlewareRegistry Create(Action<RequestMiddlewareRegistryBuilder> configure)
    {
        var builder = RequestMiddlewareRegistryBuilder.Create();
        configure(builder);

        return builder.Build();
    }
}
