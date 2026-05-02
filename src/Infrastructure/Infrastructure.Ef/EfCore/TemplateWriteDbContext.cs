using Infrastructure.Ef.Core.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Ef.EfCore;

public sealed class TemplateWriteDbContext(
    DbContextOptions<TemplateWriteDbContext> options,
    IEnumerable<IInterceptor> interceptors)
    : WriteDbContext<TemplateWriteDbContext>(options, interceptors);
