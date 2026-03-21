using Application.Abstractions.Mediator;

using Domain.Abstractions.ResultPattern;

using Infrastructure.Mediator.Wolverine.DependencyInjection;

using JasperFx;
using JasperFx.CodeGeneration;

using Wolverine.Configuration;
using Wolverine.Runtime.Handlers;

namespace Infrastructure.Mediator.Wolverine.Policies;

public sealed class AfterCommandChainPolicy(AfterCommandMiddlewareRegistry registry) : IChainPolicy
{
    public void Apply(
        IReadOnlyList<IChain> chains,
        GenerationRules rules,
        IServiceContainer container)
    {
        for (var i = 0; i < chains.Count; i++)
        {
            if (chains[i] is not HandlerChain chain)
            {
                continue;
            }

            if (!typeof(ICommand<Result>).IsAssignableFrom(chain.MessageType))
            {
                continue;
            }

            ApplyToChain(chain);
        }
    }

    private void ApplyToChain(HandlerChain chain)
    {
        foreach (var resultVariable in chain.ReturnVariablesOfType<Result>())
        {
            for (var i = 0; i < registry.MiddlewareTypes.Count; i++)
            {
                chain.Middleware.Add(
                    new AfterCommandMiddlewareFrame(
                        chain.MessageType,
                        resultVariable,
                        registry.MiddlewareTypes[i]));
            }
        }
    }
}
