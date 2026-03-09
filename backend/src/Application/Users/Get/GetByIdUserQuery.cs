using Application.Abstractions.Mediator;

using Domain.Abstractions.ResultPattern;

namespace Application.Users.Get;

public sealed record GetByIdUserQuery(Guid Id) : IQuery<Result<GetByIdUserDto>>;
