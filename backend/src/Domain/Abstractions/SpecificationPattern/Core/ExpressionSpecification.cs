using System.Linq.Expressions;

namespace Domain.Abstractions.SpecificationPattern.Core;

public sealed class ExpressionSpecification<T>(Expression<Func<T, bool>> expression) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return expression;
    }
}
