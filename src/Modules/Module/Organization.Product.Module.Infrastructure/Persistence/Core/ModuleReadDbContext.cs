using Microsoft.EntityFrameworkCore;

namespace Organization.Product.Module.Infrastructure.Persistence.Core;

public sealed class ModuleReadDbContext(
    DbContextOptions<ModuleReadDbContext> options)
    : ReadDbContext<ModuleReadDbContext>(options);
