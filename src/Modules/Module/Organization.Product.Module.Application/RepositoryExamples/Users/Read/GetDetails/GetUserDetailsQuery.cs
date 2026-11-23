namespace Organization.Product.Module.Application.RepositoryExamples.Users.Read.GetDetails;

public sealed record GetUserDetailsQuery(Guid Id)
    : IQuery<Result<UserDetailsReadModel>>;
