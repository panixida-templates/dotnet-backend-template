using Microsoft.EntityFrameworkCore;

using PANiXiDA.Core.Infrastructure.Persistence.Ef.DbContexts;

namespace Organization.Product.Module.Infrastructure.EfCore;

public sealed class TemplateReadDbContext(
    DbContextOptions<TemplateReadDbContext> options)
    : ReadDbContext<TemplateReadDbContext>(options);
