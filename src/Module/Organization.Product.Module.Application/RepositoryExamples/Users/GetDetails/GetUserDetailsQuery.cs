namespace Organization.Product.Module.Application.RepositoryExamples.Users.GetDetails;

public sealed record GetUserDetailsQuery(Guid Id)
    : IQuery<Result<UserDetailsReadModel>>;
