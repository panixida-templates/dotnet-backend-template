using Domain.Abstractions.SpecificationPattern.Core;

using System.Linq.Expressions;

namespace Domain.Abstractions.SpecificationPattern.Factories;

public static class SpecificationFactory
{
    public static Specification<T> All<T>()
    {
        return new TrueSpecification<T>();
    }

    public static Specification<T> None<T>()
    {
        return new FalseSpecification<T>();
    }

    public static Specification<T> Create<T>(Expression<Func<T, bool>> expression)
    {
        return new ExpressionSpecification<T>(expression);
    }
}
