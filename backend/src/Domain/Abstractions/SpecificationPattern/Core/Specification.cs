using Domain.Abstractions.SpecificationPattern.Abstractions;
using Domain.Abstractions.SpecificationPattern.Composition;

using System.Linq.Expressions;

namespace Domain.Abstractions.SpecificationPattern.Core;

public abstract class Specification<T> : ISpecification<T>
{
    private readonly Lazy<Func<T, bool>> compiledPredicate;

    protected Specification()
    {
        compiledPredicate = new Lazy<Func<T, bool>>(
            BuildCompiledPredicate,
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T candidate)
    {
        return compiledPredicate.Value(candidate);
    }

    public Specification<T> And(ISpecification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }

    public Specification<T> Or(ISpecification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }

    private Func<T, bool> BuildCompiledPredicate()
    {
        return ToExpression().Compile();
    }
}
