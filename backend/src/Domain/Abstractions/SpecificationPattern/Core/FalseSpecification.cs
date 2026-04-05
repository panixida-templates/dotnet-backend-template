using System.Linq.Expressions;

namespace Domain.Abstractions.SpecificationPattern.Core;

public sealed class FalseSpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return entity => false;
    }
}
