using System.Linq.Expressions;

namespace Domain.Abstractions.SpecificationPattern.Abstractions;

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T candidate);
    Expression<Func<T, bool>> ToExpression();
}
