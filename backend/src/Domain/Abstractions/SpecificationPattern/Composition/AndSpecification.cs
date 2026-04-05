using Domain.Abstractions.SpecificationPattern.Abstractions;
using Domain.Abstractions.SpecificationPattern.Expressions;

using System.Linq.Expressions;

namespace Domain.Abstractions.SpecificationPattern.Composition;

public sealed class AndSpecification<T>(ISpecification<T> left, ISpecification<T> right)
    : CompositeSpecification<T>(left, right)
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return SpecificationExpressionComposer.AndAlso(
            Left.ToExpression(),
            Right.ToExpression());
    }
}
