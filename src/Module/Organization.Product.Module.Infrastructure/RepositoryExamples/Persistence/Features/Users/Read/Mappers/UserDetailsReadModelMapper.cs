using Organization.Product.Module.Application.RepositoryExamples.Users.GetDetails;

using Riok.Mapperly.Abstractions;

namespace Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal sealed partial class UserDetailsReadModelMapper
    : IReadModelMapper<Guid, UserReadDbModel, UserDetailsReadModel>
{
    public static partial IQueryable<UserDetailsReadModel> ProjectTo(
        IQueryable<UserReadDbModel> query);
}
