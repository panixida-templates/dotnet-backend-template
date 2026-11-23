using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Organization.Product.Module.Infrastructure.Persistence.Core;

public sealed class ModuleWriteDbContext(
    DbContextOptions<ModuleWriteDbContext> options,
    IEnumerable<IInterceptor> interceptors)
    : WriteDbContext<ModuleWriteDbContext>(options, interceptors);
