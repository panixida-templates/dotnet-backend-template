using Application.Abstractions.Mediator;

using Common.ConvertParams;

using Domain.Entities;

namespace Application.Users.Get;

public sealed record GetUserQuery(Guid Id, UsersConvertParams? ConvertParams) : IQuery<User>;
