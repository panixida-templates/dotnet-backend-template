using Domain.Abstractions.SpecificationPattern.Abstractions;
using Domain.Abstractions.SpecificationPattern.Core;

namespace Domain.Abstractions.SpecificationPattern.Composition;

public abstract class CompositeSpecification<T>(ISpecification<T> left, ISpecification<T> right) 
    : Specification<T>
{
    protected ISpecification<T> Left { get; } = left;
    protected ISpecification<T> Right { get; } = right;
}
