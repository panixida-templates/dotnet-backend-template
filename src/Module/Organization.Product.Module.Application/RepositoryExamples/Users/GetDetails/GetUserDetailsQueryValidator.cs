using Organization.Product.Module.Domain.RepositoryExamples.Users;

namespace Organization.Product.Module.Application.RepositoryExamples.Users.GetDetails;

public sealed class GetUserDetailsQueryValidator : AbstractValidator<GetUserDetailsQuery>
{
    public GetUserDetailsQueryValidator()
    {
        RuleFor(query => query.Id)
            .MustBeValidDomainValue(UserId.Create);
    }
}
