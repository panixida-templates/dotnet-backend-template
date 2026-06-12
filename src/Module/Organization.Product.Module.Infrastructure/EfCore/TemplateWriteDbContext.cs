using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using PANiXiDA.Core.Infrastructure.Persistence.Ef.DbContexts;

namespace Organization.Product.Module.Infrastructure.EfCore;

public sealed class TemplateWriteDbContext(
    DbContextOptions<TemplateWriteDbContext> options,
    IEnumerable<IInterceptor> interceptors)
    : WriteDbContext<TemplateWriteDbContext>(options, interceptors);
