using PANiXiDA.Core.Application.Messaging.Mediator.Contracts;

namespace Organization.Product.Module.Application.Users.GetDetails;

public sealed record GetUserDetailsQuery(Guid Id) : IQuery<Result<UserDetailsReadModel>>;
