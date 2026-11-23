using Organization.Product.Module.Domain.RepositoryExamples.Users;

namespace Organization.Product.Module.Application.RepositoryExamples.Users.Read.GetDetails;

public sealed class GetUserDetailsQueryValidator : AbstractValidator<GetUserDetailsQuery>
{
    public GetUserDetailsQueryValidator()
    {
        RuleFor(query => query.Id)
            .MustBeValidDomainValue(UserId.Create);
    }
}
