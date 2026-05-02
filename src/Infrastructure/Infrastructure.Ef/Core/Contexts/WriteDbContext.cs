using Infrastructure.Ef.Core.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Ef.Core.Contexts;

public abstract class WriteDbContext<TDbContext>(
    DbContextOptions<TDbContext> options,
    IEnumerable<IInterceptor> interceptors) : DbContext(options)
    where TDbContext : WriteDbContext<TDbContext>
{
    protected virtual bool UseContextNameAsSchema { get; } = false;
    protected virtual bool UsePluralTableNames { get; } = true;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(interceptors);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseHiLo();

        if (UseContextNameAsSchema)
        {
            modelBuilder.HasDefaultSchema(GetSchemaName());
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TDbContext).Assembly);

        if (UsePluralTableNames)
        {
            modelBuilder.ApplyPluralTableNames();
        }
    }

    private static string GetSchemaName()
    {
        return typeof(TDbContext).ToSchemaName(
            nameof(WriteDbContext<>),
            nameof(DbContext));
    }
}
