using Domain.Abstractions.SpecificationPattern.Abstractions;
using Domain.Abstractions.SpecificationPattern.Core;
using Domain.Abstractions.SpecificationPattern.Expressions;

using System.Linq.Expressions;

namespace Domain.Abstractions.SpecificationPattern.Composition;

public sealed class NotSpecification<T>(ISpecification<T> specification)
    : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return SpecificationExpressionComposer.Not(specification.ToExpression());
    }
}
