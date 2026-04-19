using Application.Abstractions.Mediator;

namespace Infrastructure.Mediator.Wolverine.Policies.Core;

public sealed class RequestMiddlewareRegistryBuilder
{
    private readonly List<Type> beforeMiddlewareTypes = [];
    private readonly List<Type> afterMiddlewareTypes = [];
    private readonly List<Type> finallyMiddlewareTypes = [];

    public static RequestMiddlewareRegistryBuilder Create()
    {
        return new RequestMiddlewareRegistryBuilder();
    }

    public RequestMiddlewareRegistryBuilder AddBefore<TBehavior>()
    {
        return AddBefore(typeof(TBehavior));
    }

    public RequestMiddlewareRegistryBuilder AddBefore(Type behaviorType)
    {
        RequestMiddlewareRegistrationValidator.ValidateBehaviorRegistration(
            behaviorType,
            typeof(IBeforeRequestBehavior<,>),
            "Before");

        beforeMiddlewareTypes.Add(behaviorType);

        return this;
    }

    public RequestMiddlewareRegistryBuilder AddBefore(params Type[] behaviorTypes)
    {
        ArgumentNullException.ThrowIfNull(behaviorTypes);

        foreach (var behaviorType in behaviorTypes)
        {
            AddBefore(behaviorType);
        }

        return this;
    }

    public RequestMiddlewareRegistryBuilder AddAfter<TBehavior>()
    {
        return AddAfter(typeof(TBehavior));
    }

    public RequestMiddlewareRegistryBuilder AddAfter(Type behaviorType)
    {
        RequestMiddlewareRegistrationValidator.ValidateBehaviorRegistration(
            behaviorType,
            typeof(IAfterRequestBehavior<,>),
            "After");

        afterMiddlewareTypes.Add(behaviorType);

        return this;
    }

    public RequestMiddlewareRegistryBuilder AddAfter(params Type[] behaviorTypes)
    {
        ArgumentNullException.ThrowIfNull(behaviorTypes);

        foreach (var behaviorType in behaviorTypes)
        {
            AddAfter(behaviorType);
        }

        return this;
    }

    public RequestMiddlewareRegistryBuilder AddFinally<TBehavior>()
    {
        return AddFinally(typeof(TBehavior));
    }

    public RequestMiddlewareRegistryBuilder AddFinally(Type behaviorType)
    {
        RequestMiddlewareRegistrationValidator.ValidateBehaviorRegistration(
            behaviorType,
            typeof(IFinallyRequestBehavior<,>),
            "Finally");

        finallyMiddlewareTypes.Add(behaviorType);

        return this;
    }

    public RequestMiddlewareRegistryBuilder AddFinally(params Type[] behaviorTypes)
    {
        ArgumentNullException.ThrowIfNull(behaviorTypes);

        foreach (var behaviorType in behaviorTypes)
        {
            AddFinally(behaviorType);
        }

        return this;
    }

    public RequestMiddlewareRegistry Build()
    {
        return new RequestMiddlewareRegistry(
            [.. beforeMiddlewareTypes],
            [.. afterMiddlewareTypes],
            [.. finallyMiddlewareTypes]);
    }
}
