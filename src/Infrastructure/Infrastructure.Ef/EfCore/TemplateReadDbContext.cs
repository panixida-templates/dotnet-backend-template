using Infrastructure.Ef.Core.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Ef.EfCore;

public sealed class TemplateReadDbContext(
    DbContextOptions<TemplateReadDbContext> options)
    : ReadDbContext<TemplateReadDbContext>(options);
