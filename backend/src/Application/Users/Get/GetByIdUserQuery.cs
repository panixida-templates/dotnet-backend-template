using Application.Abstractions.Mediator;

namespace Application.Users.Get;

public sealed record GetByIdUserQuery(Guid Id) : IQuery<GetByIdUserDto>;
