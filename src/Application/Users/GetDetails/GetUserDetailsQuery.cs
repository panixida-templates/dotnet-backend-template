using Application.Abstractions.Mediator;

namespace Application.Users.GetDetails;

public sealed record GetUserDetailsQuery(Guid Id) : IQuery<Result<UserDetailsReadModel>>;
