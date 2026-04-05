using Domain.Abstractions.SpecificationPattern.Abstractions;
using Domain.Abstractions.SpecificationPattern.Extensions;

namespace Domain.Abstractions.SpecificationPattern.Extensions;

public static class SpecificationQueryableExtensions
{
    public static IQueryable<T> Where<T>(
        this IQueryable<T> query,
        ISpecification<T> specification)
    {
        return query.Where(specification.ToExpression());
    }

    public static IEnumerable<T> Where<T>(
        this IEnumerable<T> source,
        ISpecification<T> specification)
    {
        return source.Where(specification.IsSatisfiedBy);
    }
}
